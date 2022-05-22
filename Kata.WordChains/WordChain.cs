
namespace Kata.WordChains;

public class WordChain
{
    private readonly ExtendedWordTree _wordTree = new ();

    public void AddWords(IEnumerable<string> words)
    {
        foreach (var word in words)
        {
            _wordTree.Add(word);
        }
    }

    public bool TryWordChainGraphSearch(string from, string to, out List<string> chain)
    {
        chain = new List<string>();
        
        if (from.Length != to.Length
            || !_wordTree.Contains(from) 
            || !_wordTree.Contains(to))
            return false;

        var wordGraph = new WordGraph(StringExtensions.WordsDifferByOneLetter);
        
        foreach (var candidateWord in _wordTree.AllWordsOfLength(from.Length))
        {
            wordGraph.Add(candidateWord);
        }

        return wordGraph.TryGetShortestPath(from, to, out chain);
    }

    public bool TryWordChainNaive(string from, string to, out List<string> chain)
    {
        chain = new List<string>();
        
        if (from.Length != to.Length
            || !_wordTree.Contains(from) 
            || !_wordTree.Contains(to))
            return false;

        while (MissingIndexes().ToList() is {} indexes && indexes.Any())
        {
            if (indexes.Select(CheckIndex).FirstOrDefault(word => _wordTree.Contains(word))
                is {} candidate)
            {
                from = candidate;
                chain.Add(candidate);
            }
            else
            {
                return false;
            }
        }

        return true;

        string CheckIndex(int index)
        {
            return string.Concat(from[..index], to[index], from[(index + 1)..]);
        }

        IEnumerable<int> MissingIndexes()
        {
            var indexes = new List<int>();

            for (var i = 0; i < from.Length; i++)
            {
                if (from[i] != to[i])
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }
    }
}