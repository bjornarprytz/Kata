using System;
using System.Linq;

namespace Kata.ConflictingObjectives;

public class Extensible
{
    public string Process(string input, int wordLength, int nWords)
    {
        var wt = new WordTree();
        
        var words = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words.ShorterThan(wordLength))
        {
            wt.Add(word);
        }

        var concatenatedWords = words.OfLength(wordLength).Where(word => wt.ScanFor(word, nWords));

        return string.Join(", ", concatenatedWords);
    }
}