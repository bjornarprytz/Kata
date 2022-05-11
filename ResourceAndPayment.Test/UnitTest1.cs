using System.Linq;
using FluentAssertions;
using Kata;
using NUnit.Framework;

namespace ResourceAndPayment.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("2W", "BBW", "BBW")]
    public void PlayerPay(string cost, string resources, string expectedOutcome)
    {
        // Arrange
        var player = new Pay(cost);

        // Act
        var result = player.With(resources);

        // Assert
        result.ToList().Should().Contain(expectedOutcome.ToList());
    }
    
    
}