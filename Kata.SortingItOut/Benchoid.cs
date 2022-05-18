using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Bogus;

namespace Kata.SortingItOut;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class Benchoid
{
    private static readonly int[] Indexes = Enumerable.Range(0, 60).Shuffle(new Random(69)).ToArray();
    
    [Benchmark(Baseline = true)]
    public void NaiveRack()
    {
        Procedure(new NaiveRack60());
    }
    
    [Benchmark]
    public void BitwiseRack()
    {
        Procedure(new BitRack60());
    }
    
    [Benchmark]
    public void ArrayRack()
    {
        Procedure(new ArrayRack60());
    }
    
    [Benchmark]
    public void NaiveRack_Disorderly()
    {
        DisorderlyProcedure(new NaiveRack60());
    }
    
    [Benchmark]
    public void BitwiseRack_Disorderly()
    {
        DisorderlyProcedure(new BitRack60());
    }
    
    [Benchmark]
    public void ArrayRack_Disorderly()
    {
        DisorderlyProcedure(new ArrayRack60());
    }
    
    private void Procedure(IRack60 rack)
    {
        for (var i = 0; i < 60; i++)
        {
            rack.Add(i);
            var balls = rack.Balls.ToList();
        }
    }

    private void DisorderlyProcedure(IRack60 rack60)
    {
        foreach (var index in Indexes)
        {
            rack60.Add(index);
            var balls = rack60.Balls.ToList();
        }        
    }
}