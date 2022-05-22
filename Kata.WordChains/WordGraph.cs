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

        var openSet = new PriorityQueue<WordNode, int>();
        openSet.Enqueue(_graph[from], 0);
        var cameFrom = new Dictionary<WordNode, WordNode>();
        var gScore = new Dictionary<WordNode, int> { { _graph[from], 0 } };

        while (openSet.TryDequeue(out var current, out var priority))
        {
            if (current.Word == to)
            {
                path = ReconstructPathFrom(current).ToList();
                return true;
            }

            foreach (var neighbour in current.Neighbours)
            {
                var tentativeScore = gScore[current] + 1;
                if (gScore.ContainsKey(neighbour) && tentativeScore >= gScore[neighbour]) continue;
                
                cameFrom[neighbour] = current;
                gScore[neighbour] = tentativeScore;
                if (openSet.UnorderedItems.All(tuple => tuple.Element != neighbour))
                {
                    openSet.Enqueue(neighbour, tentativeScore);
                }
            }
        }

        return false;

        IEnumerable<string> ReconstructPathFrom(WordNode destination)
        {
            var path = new LinkedList<string>();
            path.AddFirst(destination.Word);

            var previous = destination;

            while (cameFrom!.ContainsKey(previous))
            {
                previous = cameFrom[previous];
                path.AddFirst(previous.Word);
            }
            
            return path;
        }
    }

    private class WordNode
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