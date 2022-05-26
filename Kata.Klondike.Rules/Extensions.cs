namespace Kata.Klondike.Rules;

public static class Extensions
{
    private static readonly Random Random = new ();

    private static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        var array = collection.ToArray();

        var n = array.Length;
        for (var i = 0; i < (n - 1); i++)
        {
            var r = i + Random.Next(n - i);

            (array[r], array[i]) = (array[i], array[r]);
        }

        return array;
    }

    public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> collection)
    {
        var queue = new Queue<T>(collection.Shuffle());

        while (queue.TryDequeue(out var next))
        {
            yield return next;
        }
    }

    public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}

public static class FoundationExtensions
{
    public static bool TryAddCard(this IReadOnlyDictionary<Suit, Foundation> foundations, Card card, out IReadOnlyDictionary<Suit, Foundation> newFoundations)
    {
        if (foundations[card.Suit] is { TopCard: null } && card is { Value: 1 })
        {
            newFoundations = new Dictionary<Suit, Foundation>(foundations)
            {
                [card.Suit] = new (card)
            };
            return true;
        }

        if (foundations[card.Suit] is { TopCard: not null } f && f.TopCard.Value == card.Value - 1)
        {
            newFoundations = new Dictionary<Suit, Foundation>(foundations)
            {
                [card.Suit] = new (card)
            };
            return true;
        }

        newFoundations = foundations;
        return false;
    }
}

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
}

public static class DiscardExtensions
{
    public static bool TryPopDiscard(this Discard discard, Card cardToPop, out Discard newDiscard)
    {
        if (discard.Cards.TopCardMatches(cardToPop))
        {
            newDiscard = discard with
            {
                Cards = discard.Cards.Skip(1).ToList()
            };
            return true;
        }

        newDiscard = discard;

        return false;
    }
}

public static class CollectionExtensions
{
    public static bool TopCardMatches(this IReadOnlyList<Card> collection, Card card)
    {
        return collection.Count > 0 && collection[0] == card;
    }
}