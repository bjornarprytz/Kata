namespace Kata.Klondike.Rules;

public enum Suit
{
    Clubs,
    Hearts,
    Spades,
    Diamonds
}

public record Card(int Value, Suit Suit);
public record Foundation(Card? TopCard);
public record Pile(IReadOnlyList<Card> FaceUp, int FaceDown);
public record Stock(IReadOnlyList<Card> Cards);
public record Discard(IReadOnlyList<Card> Cards);

public record GameState(IReadOnlyList<Pile> Piles, IReadOnlyDictionary<Suit, Foundation> Foundations, Stock Stock, Discard Discard, IReadOnlyList<Card> FaceDownCards, bool Victorious);
