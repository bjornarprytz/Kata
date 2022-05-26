namespace Kata.Klondike.Rules;

public static class Factory
{
    public static Pile Pile(params Card[] cards)
    {
        return new Pile(cards.ToList(), 0);
    }
    public static Pile Pile(IEnumerable<Card> cards)
    {
        return new Pile(cards.ToList(), 0);
    }

    public static Card Clubs(int value) => new (value, Suit.Clubs);
    public static Card Hearts(int value) => new (value, Suit.Hearts);
    public static Card Diamonds(int value) => new (value, Suit.Diamonds);
    public static Card Spades(int value) => new (value, Suit.Spades);
}