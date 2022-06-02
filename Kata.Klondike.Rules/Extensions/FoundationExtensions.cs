namespace Kata.Klondike.Rules;

public static class FoundationExtensions
{
    public static bool TryAddCard(this IReadOnlyDictionary<Suit, Foundation> foundations, Card card, out IReadOnlyDictionary<Suit, Foundation> newFoundations)
    {
        if (!foundations.ContainsKey(card.Suit))
        {
            newFoundations = foundations;
            return false;
        }
        
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