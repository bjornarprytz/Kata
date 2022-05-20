namespace Kata.Trigrams;

public class GramTree
{
    private readonly GramNode _root = new ();

    public void AddTrigram(string first, string second, string third)
    {
        _root.AddNextWord(first)
            .AddNextWord(second)
            .AddNextWord(third);
    }

    public string CommonThird(string first, string second)
    {
        return _root
                .Next(first)?
                .Next(second)?
                .NextWordWeighted()
                ?? string.Empty;
    }

    public string RandomThird(string first, string second)
    {
        return _root
                .Next(first)?
                .Next(second)?
                .NextWordProbably()
                ?? string.Empty;
    }
    
    
    
    private class GramNode
    {
        private readonly Dictionary<string, GramNode> _nextWords = new();
        public int Weight { get; private set; } = 1;
        public void IncreaseWeight(int w=1) => Weight += w;

        public GramNode AddNextWord(string word)
        {
            if (_nextWords.ContainsKey(word))
            {
                _nextWords[word].IncreaseWeight();
            }
            else
            {
                _nextWords.Add(word, new GramNode());
            }

            return _nextWords[word];
        }

        public string NextWordWeighted()
        {
            return _nextWords.Any() 
                ? _nextWords.Keys.MaxBy(key => _nextWords[key].Weight)!
                : string.Empty;
        }

        public string NextWordProbably()
        {
            var pick = new Random().NextSingle();

            var totalWeight = _nextWords.Values.Sum(node => node.Weight);

            var index = (int) (totalWeight * pick);

            foreach (var (word, node) in _nextWords)
            {
                index -= node.Weight;

                if (index <= 0)
                    return word;
            }
            
            return string.Empty;
        }

        public GramNode? Next(string word)
        {
            return _nextWords.ContainsKey(word)
                ? _nextWords[word]
                : null;
        }
    }
}