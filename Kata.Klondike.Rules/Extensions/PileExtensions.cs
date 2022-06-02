namespace Kata.Klondike.Rules;

public static class PileExtensions
{
    public static bool TryPopPile(this Pile pile, Card cardToPop, out Pile newPile)
    {
        if (pile.FaceUp.TopCardMatches(cardToPop))
        {
            newPile = pile with
            {
                FaceUp = pile.FaceUp.Skip(1).ToList()
            };
            
            return true;
        }

        newPile = pile;

        return false;
    }
    
    public static bool TryPopPile(this Pile pile, out Card? cardToPop, out Pile newPile)
    {
        if (pile.FaceUp.Count == 0)
        {
            cardToPop = null;
            newPile = pile;
            return false;
        }

        cardToPop = pile.FaceUp[0];
        newPile = pile with
        {
            FaceUp = pile.FaceUp.Skip(1).ToList()
        };

        return true;
    }

    public static bool TryMoveTo(this Pile sourcePile, Pile targetPile, out Pile newSourcePile, out Pile newTargetPile)
    {
        var sourceCards = sourcePile.FaceUp.ToArray();

        for (var i = 0; i < sourceCards.Length; i++)
        {
            if (!targetPile.CanAccomodate(sourceCards[i])) continue;
            
            newTargetPile = targetPile with
            {
                FaceUp = sourceCards.Take(i+1).Concat(targetPile.FaceUp).ToList()
            };
            newSourcePile = sourcePile with
            {
                FaceUp = sourceCards.Skip(i+1).ToList()
            };
            return true;
        }

        newSourcePile = sourcePile;
        newTargetPile = targetPile;

        return false;
    }
    
    public static bool TryMoveTo(this Card card, Pile targetPile, out Pile newTargetPile)
    {
        if (!targetPile.CanAccomodate(card))
        {
            newTargetPile = targetPile;
            return false;
        }

        newTargetPile = targetPile with
        {
            FaceUp = targetPile.FaceUp.Prepend(card).ToList()
        };
        return true;
    }
    
    private static bool CanAccomodate(this Pile pile, Card cardToAccomodate)
    {
        if (!pile.FaceUp.Any())
        {
            return cardToAccomodate.Value == 13;
        }
        
        var topCard = pile.FaceUp[0];
        
        return 
            topCard.Value == cardToAccomodate.Value + 1 
            && 
            topCard.Suit.OppositeColor(cardToAccomodate.Suit);
    }

    private static bool OppositeColor(this Suit suit, Suit other)
    {
        return suit.Color() != other.Color();
    }

    private static int Color(this Suit suit)
    {
        return suit switch
        {
            Suit.Diamonds => 1,
            Suit.Hearts => 1,
            Suit.Clubs => 0,
            Suit.Spades => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
        };
    }
}