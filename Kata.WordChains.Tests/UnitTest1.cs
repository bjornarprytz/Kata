using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Kata.WordChains.Tests;

public class Tests
{
    private static readonly string[] WordList = 
    {
        "cat",
        "cog",
        "dog",
        "cot",
        "goad",
        "gold",
        "load",
        "lead",
        "robs", 
        "ruby",
        "rubo",
        "rube",
        "robe",
        "rods", 
        "rubs", 
        "rode", 
        "code",
        "lengths",
        "different",
        "impossible",
        "completely",
    };

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("cat", "dog", true)]
    [TestCase("gold", "lead", true)]
    [TestCase("completely", "impossible", false)]
    [TestCase("different", "lengths", false)]
    [TestCase("non-existent", "words", false)]
    [TestCase("ruby", "code", true )]
    public void ChainWordsHasExpectedResults(string from, string to, bool expectedResult)
    {
        var wordChain = new WordChain();
        wordChain.AddWords(WordList);
        
        var result = wordChain.TryWordChainGraphSearch(from, to, out _);

        result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase("wordl", "wordy", true)]
    [TestCase("crud", "cred", true)]
    [TestCase("head", "bead", true)]
    [TestCase("head", "brad", false)]
    [TestCase("head", "heady", false)]
    public void WordsDifferByOneLetter_YieldsExpectedResult(string word1, string word2, bool expectedResult)
    {
        var result = StringExtensions.WordsDifferByOneLetter(word1, word2);

        result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase(3, 4)]
    [TestCase(9, 1)]
    [TestCase(10, 2)]
    [TestCase(69, 0)]
    [TestCase(0, 0)]
    public void WordsOfLengthNReturnCorrect(int length, int expectedCount)
    {
        var wordTree = new ExtendedWordTree();
        foreach (var word in WordList)
        {
            wordTree.Add(word);
        }

        var words = wordTree.AllWordsOfLength(length);

        words.Count().Should().Be(expectedCount);
    }
}