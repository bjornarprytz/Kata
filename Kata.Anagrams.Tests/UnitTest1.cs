using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Kata.Anagrams;

namespace Kata.Anagrams.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("a", "")]
    [TestCase("bba\nbba\nbab", "bba bab")]
    [TestCase("ab\nba", "ab ba")]
    [TestCase("fg\neh\ngf\nhe", "fg gf\neh he")]
    [TestCase("ab\naba\nba", "ab ba")]
    [TestCase("baa\naba\naab", "baa aba aab")]
    public void GroupsAnagrams(string input, string expectedOutput)
    {
        var grouper = new AnagramsGrouper(input);

        grouper.Process();

        var groupedAnagrams = grouper.DumpAnagrams();
        
        var groups = groupedAnagrams.Split('\n');
        var expectedGroups = expectedOutput.Split('\n').ToList();

        groups.Length.Should().Be(expectedGroups.Count);
        
        foreach (var group in groups)
        {
            var anagrams = group.Split(' ');

            var match = expectedGroups.FirstOrDefault(expected => expected.Split(' ').Except(anagrams).None());

            match.Should().NotBeNull();
            expectedGroups.Remove(match!);
        }
    }
}