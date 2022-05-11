using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata.ConflictingObjectives;

public class Fast : IConcatenator
{
    public string Process(string input)
    {
        var wt = new WordTree();
         
        var words = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words.Where(word => word.Length < 6))
        {
            wt.Add(word);
        }

        var concatenatedWords = words.Where(word => word.Length == 6).Where(word => wt.ScanFast(word));

        return string.Join(", ", concatenatedWords);
    }
}

public static partial class WordExtensions
{
    public static bool ScanFast(this WordTree tree, string word)
    {
        for (var i = 1; i < word.Length-1; i++)
        {
            if (tree.Contains(word[..i]) && tree.Contains(word[i..]))
            {
                return true;
            }
        }

        return false;
    }
    
    public static bool ScanFor(this WordTree tree, string word, int nWords)
    {
        if (nWords < 2 || nWords > word.Length)
            return false;

        if (nWords == word.Length)
            return word.All(c => tree.Contains(c.ToString()));
        
        return word
            .Segments(nWords - 1)
            .Any(ComposesWord);
        
        bool ComposesWord(IEnumerable<string> segments) => segments.All(tree.Contains);
    }

}