using FluentAssertions;
using NUnit.Framework;

namespace Kata.ConflictingObjectives.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("al\nbums\nalbums", "albums")]
    [TestCase("al\nbu\nms\nalbums", "")]
    [TestCase("aly\nbums\nalybums", "")]
    [TestCase("a\nbums\nabums", "")]
    [TestCase("\nasd\nal\nbums\nalbums", "albums")]
    [TestCase("\nasd\nal\nbums\nalbums\nbert\nalbert", "albums, albert")]
    public void Readable(string input, string expectedOutput)
    {
        var readable = new Readable();
        
        var result =readable.Process(input); 
        
        result.Should().Be(expectedOutput);
    }
    
    [Test]
    [TestCase("al\nbums\nalbums", "albums")]
    [TestCase("al\nbu\nms\nalbums", "")]
    [TestCase("aly\nbums\nalybums", "")]
    [TestCase("a\nbums\nabums", "")]
    [TestCase("\nasd\nal\nbums\nalbums", "albums")]
    [TestCase("\nasd\nal\nbums\nalbums\nbert\nalbert", "albums, albert")]
    public void Fast(string input, string expectedOutput)
    {
        var readable = new Fast();
        
        var result =readable.Process(input); 
        
        result.Should().Be(expectedOutput);
    }
    
    [Test]
    [TestCase("al\nbums\nalbums", 6, 2, "albums")]
    [TestCase("al\nbu\nms\nalbums", 1, 2, "")]
    [TestCase("aly\nbums\nalybums", 7, 2, "alybums")]
    [TestCase("a\nbums\nabums", 6, 2, "")]
    [TestCase("\nasd\nal\nbums\nalbums", 6, 2, "albums")]
    [TestCase("\nasd\nal\nbums\nalbums\nbert\nalbert", 6, 2, "albums, albert")]
    [TestCase("\na\ns\nd\nasd", 3, 3, "asd")]
    [TestCase("\na\ns\nda\nasda", 4, 3, "asda")]
    public void Extensible(string input, int wordLength, int nWords, string expectedOutput)
    {
        var extensible = new Extensible();

        var result = extensible.Process(input, wordLength, nWords);

        result.Should().Be(expectedOutput);
    }
}