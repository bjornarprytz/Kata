using FluentAssertions;
using NUnit.Framework;

namespace Kata.SortingItOut.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BitRack()
    {
        var rack = new BitRack60();
        
        TestProcedure(rack);
    }
    
    [Test]
    public void NaiveRack()
    {
        var rack = new NaiveRack60();
        
        TestProcedure(rack);
    }
    
    [Test]
    public void ArrayRack()
    {
        var rack = new ArrayRack60();
        
        TestProcedure(rack);
    }

    private static void TestProcedure(IRack60 rack)
    {
        rack.Add(20);
        rack.Balls.Should().ContainInOrder(20);
        rack.Add(10);
        rack.Balls.Should().ContainInOrder(10, 20);
        rack.Add(30);
        rack.Balls.Should().ContainInOrder(10, 20, 30);
    }
}