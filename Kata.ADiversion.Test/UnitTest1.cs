using System;
using FluentAssertions;
using NUnit.Framework;

namespace Kata.ADiversion.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase(1, 2)]
    [TestCase(3, 5)]
    [TestCase(4, 8)]
    [TestCase(5, 12)]
    [TestCase(10, 47)]
    public void Test1(int nDigits, int expectedNumbersWithNoAdjacent1Bits)
    {
        var result = NDigit1Bits.NumbersWithoutAdjacent1Bits(nDigits);

        result.Should().Be(expectedNumbersWithNoAdjacent1Bits);
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositiveNDigits_ShouldThrowArgumentException(int nonPositiveInt)
    {
        var callingFunctionWithNonPositiveInt =
            () => NDigit1Bits.NumbersWithoutAdjacent1Bits(nonPositiveInt);

        callingFunctionWithNonPositiveInt.Should().Throw<ArgumentException>();
    }
}