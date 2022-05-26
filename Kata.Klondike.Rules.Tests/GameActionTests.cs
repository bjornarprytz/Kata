using System;
using System.Collections.Generic;
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
        var discardedCards = new Card[]
        {
            new (1, Suit.Clubs),
            new (2, Suit.Diamonds),
        };
        
        var gameState = GameAction.Init() with
        {
            Stock = new Stock(ArraySegment<Card>.Empty),
            Discard = new Discard(discardedCards)
        };

        var nextState = gameState.PopStock();

        nextState.Discard.Cards.Count.Should().Be(0);
        nextState.Stock.Cards.Should().ContainInOrder(discardedCards);
    }
    
    [Test]
    public void PopStock_StockIsNonEmpty_TopCardIsDiscarded()
    {
        var stockCards = new Card[]
        {
            new (1, Suit.Clubs),
            new (2, Suit.Diamonds),
            new (3, Suit.Hearts),
        };

        var discardedCards = new Card[]
        {
            new (13, Suit.Spades)
        };
        
        var gameState = GameAction.Init() with
        {
            Stock = new Stock(stockCards),
            Discard = new Discard(discardedCards)
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
            Foundations = new Dictionary<Suit, Foundation>
            {
                { Suit.Hearts, new (new Card(1, Suit.Hearts)) },
                { Suit.Diamonds, new (new Card(4, Suit.Diamonds)) },
                { Suit.Clubs, new (null) },
                { Suit.Spades, new (new Card(5, Suit.Spades)) },
            },
            Piles = new Pile[]
            {
                new (new []{ cardToMove }, 0)
            }
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
        var restOfDiscard = new Card[]
        {
            new(13, Suit.Clubs),
            new(13, Suit.Diamonds),
        };
        
        var cardToMove = new Card(cardValue, cardSuit);

        var gameState = GameAction.Init() with
        {
            Foundations = new Dictionary<Suit, Foundation>
            {
                { Suit.Hearts, new (new Card(1, Suit.Hearts)) },
                { Suit.Diamonds, new (new Card(4, Suit.Diamonds)) },
                { Suit.Clubs, new (null) },
                { Suit.Spades, new (new Card(5, Suit.Spades)) },
            },
            Discard = new Discard(restOfDiscard.Prepend(cardToMove).ToList())
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
        var restOfPile = new Card[]
        {
            new(13, Suit.Clubs),
            new(13, Suit.Diamonds),
        };
        
        var cardToMove = new Card(cardValue, cardSuit);

        var gameState = GameAction.Init() with
        {
            Foundations = new Dictionary<Suit, Foundation>
            {
                { Suit.Hearts, new (new Card(1, Suit.Hearts)) },
                { Suit.Diamonds, new (new Card(4, Suit.Diamonds)) },
                { Suit.Clubs, new (null) },
                { Suit.Spades, new (new Card(5, Suit.Spades)) },
            },
            Piles = new Pile[]
            {
                new(restOfPile.Prepend(cardToMove).ToList(), 0)
            }
        };
        
        var newGameState = gameState.MoveCardToFoundation(cardToMove);

        newGameState.Piles[0].FaceUp.Should().NotContain(cardToMove);
        newGameState.Piles[0].FaceUp.Should().ContainInOrder(restOfPile);
        
        newGameState.Foundations[cardSuit].TopCard.Should().Be(cardToMove);
    }

    [Test]
    public void MoveCardFromDiscardToTableauPile_InvalidPile_StateIsUnchanged()
    {
        var restOfDiscard = new Card[]
        {
            new(13, Suit.Clubs),
            new(13, Suit.Diamonds),
        };
        
        var cardToMove = new Card(1, Suit.Clubs);

        var gameState = GameAction.Init() with
        {
            Discard = new Discard(restOfDiscard.Prepend(cardToMove).ToList()),
            
            Piles = new Pile[]
            {
                new(ArraySegment<Card>.Empty, 0)
            }
        };

        var newGameState = gameState.MoveCardFromDiscardToTableauPile(new Pile(ArraySegment<Card>.Empty, 0));

        newGameState.Should().Be(gameState);
    }
    
    [Test]
    public void MoveCardFromDiscardToTableauPile_ValidPile_CardIsMoved()
    {
        var restOfDiscard = new []
        {
            Factory.Clubs(13),
            Factory.Diamonds(13),
        };

        var cardToMove = Factory.Clubs(1);
        
        var pile = Factory.Pile(
            Factory.Diamonds(2)
        );
        
        var gameState = GameAction.Init() with
        {
            Discard = new Discard(restOfDiscard.Prepend(cardToMove).ToList()),
            Piles = new [] { pile }
        };

        var newGameState = gameState.MoveCardFromDiscardToTableauPile(pile);

        newGameState.Discard.Cards.Count.Should().Be(2);
        newGameState.Discard.Cards.Should().ContainInOrder(restOfDiscard);

        newGameState.Piles[0].FaceUp.Count.Should().Be(2);
        newGameState.Piles[0].FaceUp.Should().ContainInOrder(Factory.Diamonds(2), cardToMove);
    }
}