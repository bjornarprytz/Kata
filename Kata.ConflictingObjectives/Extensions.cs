using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata.ConflictingObjectives;

public static class Extensions
{
    private static IEnumerable<int> Range(int from, int to, int step=1)
    {
        if (step == 0)
            throw new ArgumentException();
        if (step < 0 && from < to) 
            throw new ArgumentException();
        if (step > 0 && from > to)
            throw new AggregateException();

        Func<int, bool> condition =
            step < 0
                ? x => x > to
                : x => x < to;
        
        while (condition(from))
        {
            yield return from;
            
            from += step;
        }
    }

    public static IEnumerable<IEnumerable<string>> Segments(
        this string word, int n)
    {
        var segments = new int[n+2];
        segments[0] = 0;
        segments[^1] = word.Length-1;
        
        var innerIndexes = Enumerable.Range(1, word.Length - 2).ToArray();

        foreach (var combination in innerIndexes.Combinations(n))
        {
            yield return InnerCombinations(combination);
        }

        IEnumerable<string> InnerCombinations(IEnumerable<int> indexCombination)
        {
            var i = 1;
            foreach (var itemIndex in indexCombination)
            {
                segments[i] = itemIndex;
                yield return word[segments[i - 1]..segments[i]];
                i++;
            }
        }
    }

    public static IEnumerable<T[]> Combinations<T>(
        this T[] source, int n)
    {
        if (n == source.Length)
            yield return source;
        
        if (n == 0)
            yield return Array.Empty<T>();
        
        for(var i=1; i<source.Length;i++)
        {
            foreach (var innerSequence in source[i..].Combinations(n - 1))
            {
                var items = new T[n];
                
                items[0] = source[i-1];
                Buffer.BlockCopy(innerSequence, 0, items, 1, n-1);
				
                yield return items;
            }
        }
    }
}