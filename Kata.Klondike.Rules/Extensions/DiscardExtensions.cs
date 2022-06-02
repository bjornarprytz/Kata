namespace Kata.Klondike.Rules;

public static class DiscardExtensions
{
    
    public static bool TryPopDiscard(this Discard discard, out Card? cardToPop, out Discard newDiscard)
    {
        if (!discard.Cards.Any())
        {
            cardToPop = null;
            newDiscard = discard;

            return false;    
        }

        cardToPop = discard.Cards[0];
        newDiscard = Create.Discard(discard.Cards.Skip(1).ToArray());

        return true;
    }
}