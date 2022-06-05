using System.Collections.Generic;
using FluentAssertions;
using Kata.Klondike.Rules;
using NUnit.Framework;

namespace Kata.Klondike.Strategy.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ComputeHash_DifferentStates_ResultsInDifferentHash()
    {
        var gameState = GameAction.Init();

        var gameState1 = gameState with
        {
            Piles = new[] { Create.Pile(1) }
        };
        
        var gameState2 = gameState with
        {
            Piles = new[] { Create.Pile(2) }
        };

        var hash1 = gameState1.ComputeHash();
        var hash2 = gameState2.ComputeHash();

        hash1.Should().NotBeEquivalentTo(hash2);
    }
    
    [Test]
    public void ComputeHash_IdenticalStatesFromInit_ResultsInSameHash()
    {
        var gameState = GameAction.Init();

        var gameState1 = gameState with { };

        var gameState2 = gameState with { };

        var hash1 = gameState1.ComputeHash();
        var hash2 = gameState2.ComputeHash();

        hash1.Should().BeEquivalentTo(hash2);
    }
    
    [Test]
    public void ComputeHash_IdenticalStatesFromConstructor_ResultsInSameHash()
    {
        var gameState1 = Create.Empty() with
        {
            Stock = Create.Stock(Create.Clubs(1))
        };
        var gameState2 = Create.Empty() with
        {
            Stock = Create.Stock(Create.Clubs(1))
        };

        var hash1 = gameState1.ComputeHash();
        var hash2 = gameState2.ComputeHash();

        hash1.Should().BeEquivalentTo(hash2);
    }
    
    [Test]
    public void ComputeHash_FaceDownCardsDoNotAffectHashResult()
    {
        var gameState = GameAction.Init();

        var gameState1 = gameState with
        {
            FaceDownCards = Create.List(Create.Clubs(1), Create.Diamonds(2))
        };


        var gameState2 = gameState with
        {
            FaceDownCards = Create.List( Create.Diamonds(1), Create.Spades(4))
        };

        var hash1 = gameState1.ComputeHash();
        var hash2 = gameState2.ComputeHash();

        hash1.Should().BeEquivalentTo(hash2);
    }
    
    [Test]
    public void ComputeStageHash_FaceDownCardsSameContentDifferentOrder_ResultsInSameHash()
    {
        var gameState1 = GameAction.Init() with
        {
            FaceDownCards = Create.List(
                Create.Clubs(3), 
                Create.Clubs(1), 
                Create.Clubs(2), 
                Create.Diamonds(2), 
                Create.Spades(12),
                Create.Diamonds(4), 
                Create.Spades(13), 
                Create.Spades(1), 
                Create.Diamonds(3), 
                Create.Spades(11)
            )
        };


        var gameState2 = Create.Empty() with
        {
            Stock = Create.Stock(Create.Clubs(1)),
            FaceDownCards = Create.List(
                Create.Clubs(1), 
                Create.Clubs(2), 
                Create.Clubs(3), 
                Create.Diamonds(2), 
                Create.Diamonds(3), 
                Create.Diamonds(4), 
                Create.Spades(1), 
                Create.Spades(13), 
                Create.Spades(12),
                Create.Spades(11)
            )
        };

        var hash1 = gameState1.ComputeStageHash();
        var hash2 = gameState2.ComputeStageHash();

        hash1.Should().BeEquivalentTo(hash2);
    }
    
    [Test]
    public void ComputeStageHash_FaceDownCardsDifferentContents_ResultsInDifferentHash()
    {
        var gameState = GameAction.Init();
        
        var gameState1 = gameState with
        {
            FaceDownCards = new List<Card> { Create.Clubs(1), Create.Spades(1) }
        };


        var gameState2 = gameState with
        {
            FaceDownCards = new List<Card> { Create.Clubs(1) }
        };

        var hash1 = gameState1.ComputeStageHash();
        var hash2 = gameState2.ComputeStageHash();

        hash1.Should().NotBeEquivalentTo(hash2);
    }
    
    [Test]
    public void ComputeStageHash_OnlyAffectedByFaceDownCards()
    {
        var gameState = GameAction.Init();

        var gameState1 = gameState with
        {
            FaceDownCards = new List<Card> { Create.Clubs(1), Create.Diamonds(2), Create.Spades(1) }
        };


        var gameState2 = gameState with
        {
            FaceDownCards = new List<Card> { Create.Diamonds(2), Create.Clubs(1), Create.Spades(1) }
        };

        var hash1 = gameState1.ComputeStageHash();
        var hash2 = gameState2.ComputeStageHash();

        hash1.Should().BeEquivalentTo(hash2);
    }
}