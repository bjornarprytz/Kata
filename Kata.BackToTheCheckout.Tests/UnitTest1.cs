using FluentAssertions;
using NUnit.Framework;

namespace Kata.BackToTheCheckout.Tests;

public class Tests
{
    private CheckOut _checkOut;
    
    [SetUp]
    public void Setup()
    {
        _checkOut = new CheckOut(new PricingRules().AddStandardRules());
    }

    [Test]
    [TestCase("A", 50)]
    [TestCase("B", 30)]
    [TestCase("BB", 45)]
    [TestCase("C", 20)]
    [TestCase("D", 15)]
    [TestCase("AA", 100)]
    [TestCase("AAA", 130)]
    [TestCase("AAAA", 180)]
    [TestCase("AAAAA", 230)]
    [TestCase("AAAAAA", 260)]
    [TestCase("AAAB", 160)]
    [TestCase("AAABB", 175)]
    [TestCase("AAABBD", 190)]
    [TestCase("DABABA", 190)]
    public void PricingRulesWork(string pattern, int expectedCost)
    {
        foreach (var c in pattern)
        {
            _checkOut.Scan(new Item(c.ToString()));
        }

        var result = _checkOut.Total;

        result.Should().Be(expectedCost);
    }
    
    [Test]
    [TestCase("ABAAB", 50, 80, 130, 160, 175)]
    public void IncrementalPricingRulesWork(string pattern, params int[] expectedCosts)
    {
        var i = 0;
        
        foreach (var c in pattern)
        {
            var incrementalResult = _checkOut.Scan(new Item(c.ToString()));

            incrementalResult.Should().Be(expectedCosts[i]);
            
            i++;
        }
    }
}