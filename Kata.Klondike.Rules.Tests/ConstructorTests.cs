using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Kata.Klondike.Rules.Tests;

public class ConstructorTest
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void PileConstructor_FaceUpCardsAreAscendingInOrder()
    {
        var pile = new Pile(new List<Card>
        {
            Create.Diamonds(3),
            Create.Clubs(2),
            Create.Spades(4),
        }, 0);


        pile.FaceUp.Should().ContainInOrder(
            Create.Clubs(2),
            Create.Diamonds(3),
            Create.Spades(4)
        );
    }
}