namespace Kata.Klondike.Rules;

public static class Create
{
    public static GameState Empty()
    {
        return new GameState(
            new List<Pile>(),
            new Dictionary<Suit, Foundation>(),
            new Stock(new List<Card>()),
            new Discard(new List<Card>()),
            new List<Card>(), false);
    }
    
    public static Pile Pile(int faceDownCount, params Card[] cards) => new (cards.ToList(), faceDownCount);
    public static Pile Pile(params Card[] cards) => Pile(0, cards);

    
    public static Stock Stock(params Card[] cards) => new (cards);
    public static Discard Discard(params Card[] cards) => new (cards);
    public static IReadOnlyList<T> List<T>(params T[] things) => new List<T>(things);

    public static IReadOnlyDictionary<Suit, Foundation> Foundations(
        int hearts=0, 
        int diamonds=0, 
        int clubs=0, 
        int spades=0
        )
    {
        return new Dictionary<Suit, Foundation>
        {
            [Suit.Hearts] = Heart(hearts),
            [Suit.Diamonds] = Diamond(diamonds),
            [Suit.Clubs] = Club(clubs),
            [Suit.Spades] = Spade(spades),
        };
    }
    private static Foundation Heart(int value=0) => new ( value > 0 ? Hearts(value) : null);
    private static Foundation Club(int value=0) => new ( value > 0 ? Clubs(value) : null);
    private static Foundation Diamond(int value=0) => new ( value > 0 ? Diamonds(value) : null);
    private static Foundation Spade(int value=0) => new ( value > 0 ? Spades(value) : null);

    public static Card Clubs(int value) => new (value, Suit.Clubs);
    public static Card Hearts(int value) => new (value, Suit.Hearts);
    public static Card Diamonds(int value) => new (value, Suit.Diamonds);
    public static Card Spades(int value) => new (value, Suit.Spades);
}