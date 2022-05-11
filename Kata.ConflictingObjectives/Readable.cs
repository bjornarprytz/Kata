using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata.ConflictingObjectives;

public class Readable : IConcatenator
{
    public string Process(string input)
    {
        var wt = new WordTree();
         
        var words = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words.ShorterThan(6))
        {
            wt.Add(word);
        }

        var concatenatedWords = words
            .OfLength(6)
            .Where(word => wt.ScanFast(word));

        return string.Join(", ", concatenatedWords);
    }
}

public static partial class WordExtensions
{
    public static IEnumerable<string> OfLength(this IEnumerable<string> words, int length)
    {
        return words.Where(word => word.Length == length);
    }
    
    public static IEnumerable<string> ShorterThan(this IEnumerable<string> words, int length)
    {
        return words.Where(word => word.Length < length);
    }

    public static bool IsReadableSumOf(this string word, string w1, string w2)
    {
        return
            word == w1 + w2
            ||
            word == w2 + w1;
    }
}