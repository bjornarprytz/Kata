namespace Kata.Klondike.Rules;

public static class GameAction
{
    public static GameState Init()
    {
        var cardPool = new List<Card>();
        foreach (var value in Enumerable.Range(1, 13))
        {
            cardPool.Add(new Card(value, Suit.Clubs));
            cardPool.Add(new Card(value, Suit.Diamonds));
            cardPool.Add(new Card(value, Suit.Hearts));
            cardPool.Add(new Card(value, Suit.Spades));
        }

        using var cardDealer = cardPool.RandomOrder().GetEnumerator();


        var foundations = new Dictionary<Suit, Foundation>
        {
            { Suit.Clubs, new(null) },
            { Suit.Spades, new(null) },
            { Suit.Diamonds, new(null) },
            { Suit.Hearts, new(null) }
        };

        var piles = new List<Pile>();
        foreach (var index in Enumerable.Range(0, 7))
        {
            cardDealer.MoveNext();
            piles.Add(new Pile(new[] { cardDealer.Current }, index));
        }

        var stockCards = new List<Card>();

        foreach (var _ in Enumerable.Range(0, 24))
        {
            cardDealer.MoveNext();
            stockCards.Add(cardDealer.Current);
        }

        var faceDownCards = cardDealer.ToEnumerable().ToList();

        return new GameState(piles, foundations, new Stock(stockCards), new Discard(ArraySegment<Card>.Empty),
            faceDownCards, false);
    }

    public static GameState PopStock(this GameState gameState)
    {
        if (!gameState.Stock.Cards.Any())
        {
            return gameState with
            {
                Discard = new Discard(ArraySegment<Card>.Empty),
                Stock = new Stock(gameState.Discard.Cards)
            };
        }
        
        var remainingDiscard = gameState.Stock.Cards.Take(1).Concat(gameState.Discard.Cards);
        var remainingStockCards = gameState.Stock.Cards.Skip(1);

        return gameState with
        {
            Discard = new Discard(remainingDiscard.ToList()),
            Stock = new Stock(remainingStockCards.ToList())
        };
    }

    public static GameState MoveCardFromDiscardToFoundation(this GameState gameState)
    {
        if (!gameState.Discard.TryPopDiscard(out var card, out var newDiscard))
            return gameState;
        
        if (!gameState.Foundations.TryAddCard(card!, out var newFoundations))
        {
            return gameState;
        }
        
        return gameState with
        {
            Discard = newDiscard,
            Foundations = newFoundations
        };
    }
    
    public static GameState MoveCardFromPileToFoundation(this GameState gameState, Pile pile)
    {
        if (!pile.TryPopPile(out var card, out var newPile))
            return gameState;
        
        if (!gameState.Foundations.TryAddCard(card!, out var newFoundations))
        {
            return gameState;
        }

        var index = gameState.Piles.IndexOf(pile);

        if (index == -1)
            return gameState;
        
        return 
            CheckForVictory(CheckEmptyPiles(
                gameState with
                {
                    Piles = new List<Pile>(gameState.Piles)
                    {
                        [index] = newPile
                    },
                    Foundations = newFoundations
                }));

        return gameState;
    }

    public static GameState MoveCardFromDiscardToTableauPile(this GameState gameState, Pile pile)
    {
        /*
         * Move the top card of the discard pile to one of the tableau piles.
         * This card must be one less in rank and opposite in color to the card at the top of the destination tableau.
         */
        
        if (!gameState.Discard.TryPopDiscard(out var cardToMove, out var newDiscard))
        {
            return gameState;
        }

        var pileIndex = gameState.Piles.IndexOf(pile);

        if (pileIndex == -1 || !cardToMove!.TryMoveTo(gameState.Piles[pileIndex], out var newTargetPile))
            return gameState;
        
        return gameState with
        {
            Piles = new List<Pile>(gameState.Piles)
            {
                [pileIndex] = newTargetPile
            },
            Discard = newDiscard
        };
    }

    public static GameState MovePileToOtherPile(this GameState gameState, Pile source, Pile target)
    {
        /*
         * Move one or more cards from one tableau pile to another. If multiple cards are moved, they
         * must be a sequence ascending in rank and alternating in color. The card moved (or the top of
         * the sequence moved) must be one less in rank and opposite in color to the card at the top of
         * the destination tableau. If the move leaves a face-down card to the top of the original pile, turn it over.
         */
        
        var sourcePileIndex = gameState.Piles.IndexOf(source);
        var targetPileIndex = gameState.Piles.IndexOf(target);

        if (sourcePileIndex == -1 || targetPileIndex == -1)
            return gameState;

        if (source.TryMoveTo(target, out var newSourcePile, out var newTargetPile))
        {
            return CheckForVictory(CheckEmptyPiles(
                gameState with
                {
                    Piles = new List<Pile>(gameState.Piles)
                    {
                        [sourcePileIndex] = newSourcePile,
                        [targetPileIndex] = newTargetPile
                    }
                }));
        }

        return gameState;
    }

    private static GameState CheckForVictory(GameState gameState)
    {
        if (gameState.FaceDownCards.Count == 0) // Simplified victory condition because completing foundations is a formality at this stage
            return gameState with { Victorious = true };

        return gameState;
    }

    private static GameState CheckEmptyPiles(GameState gameState)
    {
        using var cardDealer = gameState.FaceDownCards.RandomOrder().GetEnumerator();
        
        foreach (var (pile, index) in gameState.Piles.Select((p, i) => (p, i)))
        {
            if (pile.FaceUp.Count > 0 || pile.FaceDown == 0)
                continue;

            if (!cardDealer.MoveNext())
                throw new InvalidOperationException("Exhausted cards while checking empty piles");
            
            var newPile = new Pile(new []{ cardDealer.Current }, pile.FaceDown - 1);
            
            gameState = gameState with
            {
                Piles = new List<Pile>(gameState.Piles)
                {
                    [index] = newPile
                },
            };
        }

        return gameState with { FaceDownCards = cardDealer.ToEnumerable().ToList() };
    }
    
    
}