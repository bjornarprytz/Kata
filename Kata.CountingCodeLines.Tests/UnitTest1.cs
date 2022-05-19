using FluentAssertions;
using NUnit.Framework;

namespace Kata.CountingCodeLines.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase(
        @"
            // This file contains 3 lines of code
            public interface Dave {
                /**
                * count the number of lines in a file
                */
                int countLines(File inFile); // not the real signature!
            }
        ",
        3
        )]
    
    [TestCase(
        @"
            /*****
            * This is a test program with 5 lines of code
            *  \/* no nesting allowed!
            //*****//***/// Slightly pathological comment ending...

            public class Hello {
                public static final void main(String [] args) { // gotta love Java
                    // Say hello
                  System./*wait*/out./*for*/println/*it*/(""Hello/*"");
                 }

            }
        ",
        5
    )]
    public void Test1(string code, int expectedLineCount)
    {
        var nLinesOfCode = LineCounter.Count(code);

        nLinesOfCode.Should().Be(expectedLineCount);
    }
}