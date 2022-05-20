using System.Text;

namespace Kata.Trigrams;

public class Trigrams
{
    private readonly GramTree _gramTree = new ();

    public void Analyse(string corpus)
    {
        const int nGramSize = 3;
        
        var window = new Queue<string>(nGramSize);

        foreach (var line in corpus.Split("\r\n"))
        {
            var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var token in tokens)
            {
                window.Enqueue(token);

                if (window.Count != nGramSize) continue;
                
                _gramTree.AddTrigram(
                    window.ElementAt(0),
                    window.ElementAt(1),
                    window.ElementAt(2)
                );

                window.Dequeue();
            }
        }
    }

    public string CommonThirdWord(string first, string second)
    {
        return _gramTree.CommonThird(first, second);
    }
    
    public string RandomThirdWord(string first, string second)
    {
        return _gramTree.RandomThird(first, second);
    }

    public string PredictNWordNovelFrom(int nWords, string first, string second)
    {
        var sb = new StringBuilder();

        var currentLine = $"{first} {second}";

        while (nWords > 0 && RandomThirdWord(first, second) is {} word&& word.Any())
        {
            currentLine += $" {word}";
            
            if (currentLine.Length > 50)
            {
                sb.AppendLine(currentLine);
                currentLine = string.Empty;
            }

            first = second;
            second = word;
            nWords--;
        }

        return sb.ToString();
    }
}