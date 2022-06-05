using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Kata.Klondike.Rules;
using NUnit.Framework;

namespace Kata.Klondike.Strategy.Tests;


public class KlondikePlayerTests
{
    private IKlondikePlayer[] _klondikePlayers;
    
    [SetUp]
    public void Setup()
    {
        _klondikePlayers = new IKlondikePlayer[]
        {
            new ExhaustiveKlondikePlayer(),
            new SystematicKlondikePlayer()
        };
    }

    
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void TryToWin_MultipleUnknowns_FindsWin(int playerIndex)
    {
        var klondikePlayer = _klondikePlayers[playerIndex];
        
        var gameState = Create.Empty() with
        {
            Foundations = Create.Foundations(spades: 12, hearts: 11, clubs: 12, diamonds: 12),
            Piles = Create.List(
                Create.Pile(2,Create.Spades(13), Create.Hearts(12))
            ),
            FaceDownCards = Create.List(Create.Clubs(13), Create.Diamonds(13)) 
        };
        
        var finalState = klondikePlayer.TryToWin(gameState);

        finalState.Victorious.Should().BeTrue();
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void TryToWin_MultipleMoves_FindsWin(int playerIndex)
    {
        var klondikePlayer = _klondikePlayers[playerIndex];
        var gameState = Create.Empty() with
        {
            Foundations = Create.Foundations(spades: 9, hearts: 9, clubs: 12, diamonds: 12),
            Piles = Create.List(
                Create.Pile(1,Create.Hearts(12), Create.Spades(13)),
                Create.Pile(1,Create.Spades(12), Create.Hearts(13)),
                Create.Pile(0,Create.Hearts(10), Create.Spades(11)),
                Create.Pile(0,Create.Spades(10), Create.Hearts(11))
            ),
            FaceDownCards = Create.List(Create.Clubs(13), Create.Diamonds(13)) 
        };
        
        var finalState = klondikePlayer.TryToWin(gameState);

        finalState.Victorious.Should().BeTrue();
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void TryToWin_UnwinnableState_FindsLoss(int playerIndex)
    {
        var klondikePlayer = _klondikePlayers[playerIndex];
        
        var gameState = Create.Empty() with
        {
            Foundations = Create.Foundations(spades: 13, hearts: 11, clubs: 13, diamonds: 13),
            Piles = Create.List(
                Create.Pile(1,Create.Hearts(13))
            ),
            FaceDownCards = Create.List(Create.Hearts(12)) 
        };
        
        var finalState = klondikePlayer.TryToWin(gameState);

        finalState.Victorious.Should().BeFalse();
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void TryToWin_SomeCardInStock_FindsWin(int playerIndex)
    {
        var klondikePlayer = _klondikePlayers[playerIndex];
        
        var gameState = Create.Empty() with
        {
            Foundations = Create.Foundations(spades: 9, hearts: 9, clubs: 12, diamonds: 12),
            Piles = Create.List(
                Create.Pile(1,Create.Hearts(12)),
                Create.Pile(1,Create.Spades(12)),
                Create.Pile(0,Create.Hearts(10)),
                Create.Pile(0,Create.Spades(10))
            ),
            Stock = Create.Stock(Create.Hearts(11), Create.Spades(11), Create.Hearts(13), Create.Spades(13)),
            FaceDownCards = Create.List(Create.Clubs(13), Create.Diamonds(13)) 
        };
        
        var finalState = klondikePlayer.TryToWin(gameState);

        finalState.Victorious.Should().BeTrue();
    }
}