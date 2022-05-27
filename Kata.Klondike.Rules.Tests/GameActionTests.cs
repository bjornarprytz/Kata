using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Kata.Klondike.Rules.Tests;

public class GameActionTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void InitGameState_StateIsCorrect()
    {
        var gameState = GameAction.Init();

        gameState.Foundations.Count.Should().Be(4);

        foreach (var suit in typeof(Suit).GetEnumValues().Cast<Suit>())
        {
            gameState.Foundations[suit].TopCard.Should().BeNull();
        }

        gameState.Piles.Count.Should().Be(7);
        foreach (var (pile, index) in gameState.Piles.Select(((pile, index) => (pile, index))))
        {
            pile.FaceDown.Should().Be(index);
            pile.FaceUp.Count.Should().Be(1);
        }

        gameState.Discard.Cards.Count.Should().Be(0);
        gameState.Stock.Cards.Count.Should().Be(24);

        gameState.FaceDownCards.Count.Should().Be(21);
    }

    [Test]
    public void PopStock_StockIsEmpty_DiscardIsFlipped()
    {
        var discardedCards = new []
        {
            Create.Clubs(1),
            Create.Diamonds(2)
        };
        
        var gameState = GameAction.Init() with
        {
            Stock = Create.Stock(),
            Discard = Create.Discard(discardedCards)
        };

        var nextState = gameState.PopStock();

        nextState.Discard.Cards.Count.Should().Be(0);
        nextState.Stock.Cards.Should().ContainInOrder(discardedCards);
    }
    
    [Test]
    public void PopStock_StockIsNonEmpty_TopCardIsDiscarded()
    {
        var stockCards = new []
        {
            Create.Clubs(1),
            Create.Diamonds(2),
            Create.Hearts(3)
        };

        var discardedCards = new [] { Create.Spades(13) };
        
        var gameState = GameAction.Init() with
        {
            Stock = Create.Stock(stockCards),
            Discard = Create.Discard(discardedCards)
        };

        var nextState = gameState.PopStock();

        nextState.Discard.Cards.Count.Should().Be(2);
        nextState.Discard.Cards.Should().ContainInOrder(discardedCards.Prepend(stockCards[0]));
        
        nextState.Stock.Cards.Count.Should().Be(2);
        nextState.Stock.Cards.Should().ContainInOrder(stockCards[1..]);
    }
    
    [Test]
    [TestCase(1, Suit.Hearts)]
    [TestCase(13, Suit.Diamonds)]
    [TestCase(3, Suit.Clubs)]
    [TestCase(13, Suit.Spades)]
    public void MoveCardToFoundation_NoSuitableFoundation_StateIsUnchanged(int cardValue, Suit cardSuit)
    {
        var cardToMove = new Card(cardValue, cardSuit);

        var gameState = GameAction.Init() with
        {
            Foundations = Create.Foundations(1, 4, 0, 5),
            Piles = new [] { Create.Pile(cardToMove) }
        };

        var newGameState = gameState.MoveCardToFoundation(cardToMove);

        newGameState.Should().Be(gameState);
    }
    
    [Test]
    [TestCase(2, Suit.Hearts)]
    [TestCase(5, Suit.Diamonds)]
    [TestCase(1, Suit.Clubs)]
    [TestCase(6, Suit.Spades)]
    public void MoveCardToFoundation_CardMovesFromDiscardToSuitableFoundation(int cardValue, Suit cardSuit)
    {
        var restOfDiscard = new []
        {
            Create.Clubs(13),
            Create.Diamonds(13)
        };
        
        var cardToMove = new Card(cardValue, cardSuit);

        var discard = restOfDiscard.Prepend(cardToMove).ToArray();

        var gameState = GameAction.Init() with
        {
            Foundations = Create.Foundations(1, 4, 0, 5),
            Discard = Create.Discard(discard)
        };

        var newGameState = gameState.MoveCardToFoundation(cardToMove);

        newGameState.Discard.Cards.Should().NotContain(cardToMove);
        newGameState.Discard.Cards.Should().ContainInOrder(restOfDiscard);
        
        newGameState.Foundations[cardSuit].TopCard.Should().Be(cardToMove);
    }
    
    [Test]
    [TestCase(2, Suit.Hearts)]
    [TestCase(5, Suit.Diamonds)]
    [TestCase(1, Suit.Clubs)]
    [TestCase(6, Suit.Spades)]
    public void MoveCardToFoundation_CardMovesFromPileToSuitableFoundation(int cardValue, Suit cardSuit)
    {
        var restOfPile = new []
        {
            Create.Clubs(13),
            Create.Diamonds(13)
        };
        
        var cardToMove = new Card(cardValue, cardSuit);

        var pile = Create.Pile(restOfPile.Prepend(cardToMove).ToArray());

        var gameState = GameAction.Init() with
        {
            Foundations = Create.Foundations(1, 4, 0, 5),
            Piles = new [] { pile }
        };
        
        var newGameState = gameState.MoveCardToFoundation(cardToMove);

        newGameState.Piles[0].FaceUp.Should().NotContain(cardToMove);
        newGameState.Piles[0].FaceUp.Should().ContainInOrder(restOfPile);
        
        newGameState.Foundations[cardSuit].TopCard.Should().Be(cardToMove);
    }

    [Test]
    public void MoveCardFromDiscardToTableauPile_InvalidPile_StateIsUnchanged()
    {
        var restOfDiscard = new []
        {
            Create.Clubs(13),
            Create.Diamonds(13),
        };
        
        var cardToMove = new Card(1, Suit.Clubs);

        var invalidPile = Create.Pile();

        var gameState = GameAction.Init() with
        {
            Discard = Create.Discard(restOfDiscard.Prepend(cardToMove).ToArray()),
            
            Piles = new [] { invalidPile }
        };

        var newGameState = gameState.MoveCardFromDiscardToTableauPile(invalidPile);

        newGameState.Should().Be(gameState);
    }
    
    [Test]
    public void MoveCardFromDiscardToTableauPile_MoveKingOntoEmptyPileIsPossible()
    {
        var cardToMove = Create.Clubs(13);

        var emptyPile = Create.Pile();

        var gameState = GameAction.Init() with
        {
            Discard = Create.Discard(cardToMove),
            
            Piles = new [] { emptyPile }
        };

        var newGameState = gameState.MoveCardFromDiscardToTableauPile(emptyPile);

        newGameState.Piles[0].FaceUp.Should().Contain(cardToMove);
        newGameState.Discard.Cards.Should().NotContain(cardToMove);
    }
    
    [Test]
    public void MoveCardFromDiscardToTableauPile_ValidPile_CardIsMoved()
    {
        var restOfDiscard = new []
        {
            Create.Clubs(13),
            Create.Diamonds(13),
        };

        var cardToMove = Create.Clubs(1);
        
        var pile = Create.Pile(Create.Diamonds(2));
        
        var gameState = GameAction.Init() with
        {
            Discard = new Discard(restOfDiscard.Prepend(cardToMove).ToList()),
            Piles = new [] { pile }
        };

        var newGameState = gameState.MoveCardFromDiscardToTableauPile(pile);

        newGameState.Discard.Cards.Count.Should().Be(2);
        newGameState.Discard.Cards.Should().ContainInOrder(restOfDiscard);

        newGameState.Piles[0].FaceUp.Count.Should().Be(2);
        newGameState.Piles[0].FaceUp.Should().ContainInOrder(
            cardToMove, 
            Create.Diamonds(2)
            );
    }
    
    [Test]
    public void MoveCardFromPileToFoundation_FinalFaceUpCardMoved_FaceDownCardIsFlippedUp()
    {
        var cardToMove = Create.Clubs(1);
        
        var pile = Create.Pile(2, cardToMove);
        
        var gameState = GameAction.Init() with
        {
            Piles = new [] { pile }
        };

        var faceDownCards = gameState.FaceDownCards.ToList();
        
        var newGameState = gameState.MoveCardToFoundation(cardToMove);

        newGameState.FaceDownCards.Count.Should().Be(faceDownCards.Count - 1);
        newGameState.Piles[0].FaceDown.Should().Be(pile.FaceDown - 1);
        
        newGameState.Piles[0].FaceUp.Should().HaveCount(1);
        newGameState.Piles[0].FaceUp.Should().NotContain(cardToMove);

        faceDownCards.Any(c => c == newGameState.Piles[0].FaceUp[0]).Should().BeTrue();
    }
    
    [Test]
    public void MovePileToOtherPile_FinalFaceUpCardMoved_FaceDownCardIsFlippedUp()
    {
        var cardToMove = Create.Clubs(2);
        var topCardInTargetPile = Create.Diamonds(3);
        
        var pile = Create.Pile(2, cardToMove);
        var targetPile = Create.Pile(topCardInTargetPile);
        
        var gameState = GameAction.Init() with
        {
            Piles = new [] { pile, targetPile }
        };

        var faceDownCards = gameState.FaceDownCards.ToList();

        var newGameState = gameState.MovePileToOtherPile(pile, targetPile);

        newGameState.FaceDownCards.Count.Should().Be(faceDownCards.Count - 1);
        newGameState.Piles[0].FaceDown.Should().Be(pile.FaceDown - 1);
        
        newGameState.Piles[0].FaceUp.Should().HaveCount(1);
        newGameState.Piles[0].FaceUp.Should().NotContain(cardToMove);

        faceDownCards.Any(c => c == newGameState.Piles[0].FaceUp[0]).Should().BeTrue();
    }

    [Test]
    public void MovePileToOtherPile_WholePileMoved()
    {
        var cardsToMove = new []
        {
            Create.Clubs(4),
            Create.Diamonds(5),
            Create.Spades(6),
        };
        var topCardInTargetPile = Create.Diamonds(7);
        
        var pileToMove = Create.Pile(cardsToMove);
        var targetPile = Create.Pile(topCardInTargetPile);
        
        var gameState = GameAction.Init() with
        {
            Piles = new [] { pileToMove, targetPile }
        };

        var newGameState = gameState.MovePileToOtherPile(pileToMove, targetPile);

        newGameState.Piles[0].FaceUp.Should().HaveCount(0);
        newGameState.Piles[1].FaceUp.Should().HaveCount(4);
        newGameState.Piles[1].FaceUp.Should().ContainInOrder(
            Create.Clubs(4),
            Create.Diamonds(5),
            Create.Spades(6),
            Create.Diamonds(7)
            );
    }
    
    [Test]
    public void MovePileToOtherPile_PartsOfPileMoved()
    {
        var cardsToMove = new []
        {
            Create.Clubs(4),
            Create.Diamonds(5),
            Create.Spades(6),
        };
        var topCardInTargetPile = Create.Clubs(6);
        
        var pileToMove = Create.Pile(cardsToMove);
        var targetPile = Create.Pile(topCardInTargetPile);
        
        var gameState = GameAction.Init() with
        {
            Piles = new [] { pileToMove, targetPile }
        };

        var newGameState = gameState.MovePileToOtherPile(pileToMove, targetPile);

        newGameState.Piles[0].FaceUp.Should().HaveCount(1);
        newGameState.Piles[1].FaceUp.Should().HaveCount(3);
        newGameState.Piles[1].FaceUp.Should().ContainInOrder(
                Create.Clubs(4),
                Create.Diamonds(5),
                Create.Clubs(6)
            );
    }
    
    [Test]
    public void MovePileToOtherPile_MoveKingOntoEmptyPileIsPossible()
    {
        var cardToMove = Create.Clubs(13);

        var sourcePile = Create.Pile(cardToMove);
        var emptyTargetPile = Create.Pile();

        var gameState = GameAction.Init() with
        {
            Piles = new [] { sourcePile, emptyTargetPile }
        };

        var newGameState = gameState.MovePileToOtherPile(sourcePile, emptyTargetPile);

        newGameState.Piles[0].FaceUp.Should().NotContain(cardToMove);
        newGameState.Piles[1].FaceUp.Should().Contain(cardToMove);
    }
}