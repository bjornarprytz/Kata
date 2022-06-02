namespace Kata.Klondike.Rules;

public enum Suit
{
    Clubs,
    Hearts,
    Spades,
    Diamonds
}

public record Card(int Value, Suit Suit)
{
    public class Comparer : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            var suitComparison = x.Suit.CompareTo(y.Suit); 
            return suitComparison != 0 ? suitComparison : x.Value.CompareTo(y.Value);
        }
    }
};
public record Foundation(Card? TopCard);
public record Pile(IReadOnlyList<Card> FaceUp, int FaceDown);
public record Stock(IReadOnlyList<Card> Cards);
public record Discard(IReadOnlyList<Card> Cards);

public record GameState(IReadOnlyList<Pile> Piles, IReadOnlyDictionary<Suit, Foundation> Foundations, Stock Stock, Discard Discard, IReadOnlyList<Card> FaceDownCards, bool Victorious);
