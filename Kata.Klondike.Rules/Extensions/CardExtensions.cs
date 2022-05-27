namespace Kata.Klondike.Rules;

public static class CardExtensions
{
    public static bool TopCardMatches(this IReadOnlyList<Card> collection, Card card)
    {
        return collection.Count > 0 && collection[0] == card;
    }

    
}