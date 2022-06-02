using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Kata.Klondike.Rules;
using NUnit.Framework;

namespace Kata.Klondike.Strategy.Tests;


public class KlondikePlayerTests
{
    private KlondikePlayer _klondikePlayer;
    
    [SetUp]
    public void Setup()
    {
        _klondikePlayer = new KlondikePlayer();
    }

    [Test]
    public void MakeNextMove_ImpossibleToProgress_NoAvailableTransitions()
    {
        var gameState = Create.Empty() with
        {
            Piles = Create.List(Create.Pile(1,Create.Hearts(12))),
            FaceDownCards = Create.List(Create.Clubs(13)) 
        };
        
        var transitions = _klondikePlayer.FindTransitionsToNextStage(gameState);

        transitions.Should().BeEmpty();
    }
    
    [Test]
    public void MakeNextMove_PossibleToProgress_FindOneTransition()
    {
        var gameState = Create.Empty() with
        {
            Piles = Create.List(
                Create.Pile(1,Create.Hearts(12)),
                Create.Pile(Create.Spades(13))
                
                ),
            FaceDownCards = Create.List(Create.Clubs(13)) 
        };
        
        var transitions = _klondikePlayer.FindTransitionsToNextStage(gameState).ToList();

        transitions.Should().HaveCount(1);

        var nextGameState = transitions[0].ActionExpression.Compile().Invoke(gameState);

        nextGameState.Piles.Should().HaveCount(2);
        nextGameState.Piles[0].FaceDown.Should().Be(0);
        nextGameState.Piles[0].FaceUp.Should().Contain(Create.Clubs(13));

        nextGameState.Piles[1].FaceUp.Should().HaveCount(2);
        nextGameState.Piles[1].FaceUp.Should().Contain(Create.Spades(13));
        nextGameState.Piles[1].FaceUp.Should().Contain(Create.Hearts(12));
    }
    
    [Test]
    public void MakeNextMove_PossibleToProgress_FindTwoTransitions()
    {
        var gameState = Create.Empty() with
        {
            Foundations = Create.Foundations(spades: 12, hearts: 11),
            Piles = Create.List(
                Create.Pile(1,Create.Spades(13), Create.Hearts(12))
            ),
            FaceDownCards = Create.List(Create.Clubs(13)) 
        };
        
        var transitions = _klondikePlayer.FindTransitionsToNextStage(gameState).ToList();

        transitions.Should().HaveCount(2);

        var currentState = gameState;
        
        foreach (var stateTransition in transitions)
        {
            currentState = stateTransition.ActionExpression.Compile().Invoke(currentState);
        }

        currentState.Piles.Should().HaveCount(1);
        currentState.Piles[0].FaceDown.Should().Be(0);
        currentState.Piles[0].FaceUp.Should().Contain(Create.Clubs(13));

        currentState.Foundations[Suit.Spades].TopCard.Should().NotBeNull();
        currentState.Foundations[Suit.Spades].TopCard!.Value.Should().Be(13);
        
        currentState.Foundations[Suit.Hearts].TopCard.Should().NotBeNull();
        currentState.Foundations[Suit.Hearts].TopCard!.Value.Should().Be(12);
    }
}