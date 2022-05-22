namespace Kata.WordChains;

public interface IGraphNode<out T>
where T : IGraphNode<T>
{
    IEnumerable<T> Neighbours { get; }
}

public static class NodeExtensions
{
    public static bool AStarTo<TNode>(this TNode from, TNode to, out List<TNode> path) 
        where TNode : class, IGraphNode<TNode>
    {
        path = new List<TNode>();

        var openSet = new PriorityQueue<TNode, int>();
        openSet.Enqueue(from, 0);
        var cameFrom = new Dictionary<TNode, TNode>();
        var gScore = new Dictionary<TNode, int> { { from, 0 } };

        while (openSet.TryDequeue(out var current, out var priority))
        {
            if (current == to)
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

        IEnumerable<TNode> ReconstructPathFrom(TNode destination)
        {
            var path = new LinkedList<TNode>();
            path.AddFirst(destination);

            var previous = destination;

            while (cameFrom!.ContainsKey(previous))
            {
                previous = cameFrom[previous];
                path.AddFirst(previous);
            }
            
            return path;
        }
    }
}