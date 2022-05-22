using Kata.ConflictingObjectives;

namespace Kata.WordChains;

public class WordGraph
{
    private readonly Dictionary<string, WordNode> _graph = new ();

    private readonly Func<string, string, bool> _adjacencyFunc;

    public WordGraph(Func<string, string, bool> adjacencyFunc)
    {
        _adjacencyFunc = adjacencyFunc;
    }

    public void Add(string word)
    {
        if (_graph.ContainsKey(word))
            return;

        var node = new WordNode(word);

        foreach (var adjacentNode in _graph.Values.Where(n => _adjacencyFunc(n.Word, word)))
        {
            adjacentNode.AddConnection(node);
            node.AddConnection(adjacentNode);
        }
        
        _graph.Add(node.Word, node);
    }

    public bool TryGetShortestPath(string from, string to, out List<string> path)
    {
        path = new List<string>();


        if (_graph.ContainsKey(from) && _graph.ContainsKey(to)
            &&
            _graph[from].AStarTo(_graph[to], out var nodePath)
        )
        {
            path = nodePath.Select(n => n.Word).ToList();
            return true;
        }
        
        return false;
        

    }

    private class WordNode : IGraphNode<WordNode>
    {
        private readonly Dictionary<string, WordNode> _connections = new();

        public WordNode(string word)
        {
            Word = word;
        }
        public string Word { get; }
        public IEnumerable<WordNode> Neighbours => _connections.Values;
        
        public void AddConnection(WordNode otherNode)
        {
            if (_connections.ContainsKey(otherNode.Word))
                return;
            
            _connections.Add(otherNode.Word, otherNode);
        }

    }
}