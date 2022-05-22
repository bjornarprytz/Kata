using System.Linq;
using FluentAssertions;
using Kata.TransitiveDependencies;
using NUnit.Framework;

namespace TransitiveDependencies.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    [TestCase('a', 'b', 'c', 'e', 'f', 'g', 'h')]
    [TestCase('b', 'c', 'e', 'f', 'g', 'h')]
    [TestCase('c', 'g')]
    [TestCase('d', 'a', 'b', 'c', 'e', 'f', 'g', 'h')]
    [TestCase('e', 'f', 'h')]
    [TestCase('f', 'h')]
    public void TransitiveDependencies(char thing, params char[] expectedDependencies)
    {
        var dGraph = new DependencyGraph();
        dGraph.Add('a', 'b', 'c');
        dGraph.Add('b', 'c', 'e');
        dGraph.Add('c', 'g');
        dGraph.Add('d', 'a', 'f');
        dGraph.Add('e', 'f');
        dGraph.Add('f', 'h');
        
        var dependencies = dGraph.DependenciesFor(thing).OrderBy(t => t).ToList();

        dependencies.Count.Should().Be(expectedDependencies.Length);
        dependencies.Should().ContainInOrder(expectedDependencies.OrderBy(t => t));
    }


    [Test]
    [TestCase('a', 'b', 'c')]
    [TestCase('b', 'a', 'c')]
    [TestCase('c', 'a', 'b')]
    public void CircularDependencies(char thing, params char[] expectedDependencies)
    {
        var dGraph = new DependencyGraph();
        dGraph.Add('a', 'b');
        dGraph.Add('b', 'c');
        dGraph.Add('c', 'a');
        
        var dependencies = dGraph.DependenciesFor(thing).OrderBy(t => t).ToList();
        
        dependencies.Count.Should().Be(expectedDependencies.Length);
        dependencies.Should().ContainInOrder(expectedDependencies.OrderBy(t => t));
    }
}