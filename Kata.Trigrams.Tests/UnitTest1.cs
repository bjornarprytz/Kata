using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Kata.Trigrams.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("How", "I", "met")]
    [TestCase("your", "mother", "")]
    [TestCase("met", "your", "mother")]
    public void CommonThirdWordMatches(string first, string second, string expectedNextWord)
    {
        const string corpus = "How I met your mother";

        var gram = new Trigrams();
        
        gram.Analyse(corpus);

        var nextWord = gram.CommonThirdWord(first, second);

        nextWord.Should().Be(expectedNextWord);
    }
    
    [Test]
    [TestCase("I", "may", "join")]
    [TestCase("may", "join", "I")]
    public void WeightOfWordsMatter(string first, string second, string expectedNextWord)
    {
        const string corpus = "I may not I may join I may join";

        var gram = new Trigrams();
        
        gram.Analyse(corpus);

        var nextWord = gram.CommonThirdWord(first, second);

        nextWord.Should().Be(expectedNextWord);
    }
}