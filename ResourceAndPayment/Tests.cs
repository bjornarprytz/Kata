using System;
using NUnit.Framework;
using Kata;

namespace ResourceAndPayment
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("2W", "BBW", true)]
        public void PlayerPay(string cost, string resources, bool expectedOutcome)
        {
            // Arrange
            var player = new Player();

            // Act

            // Assert
        }
    }
}