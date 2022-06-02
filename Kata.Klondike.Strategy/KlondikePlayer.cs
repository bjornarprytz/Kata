using Kata.Klondike.Rules;

namespace Kata.Klondike.Strategy;

public class KlondikePlayer
{
    public GameState TryToWin(GameState gameState)
    {
        var currentStage = gameState;

        var nextMoves = FindTransitionsToNextStage(currentStage).ToList();

        while (nextMoves.Any())
        {
            foreach (var move in nextMoves)
            {
                currentStage = move.TransitionMove.Invoke(currentStage);
            }

            nextMoves = FindTransitionsToNextStage(currentStage).ToList();
        }

        return currentStage;
    }
    
    public IEnumerable<StateTransition> FindTransitionsToNextStage(GameState currentGameState)
    {
        var originStageHash = currentGameState.ComputeStageHash();
        var originHash = currentGameState.ComputeHash();

        var knownStates = new Dictionary<string, GameState>{{originHash, currentGameState}};
        var openSet = new PriorityQueue<StateTransition, int>();

        foreach (var transition in currentGameState.Neighbours(ref knownStates))
        {
            openSet.Enqueue(transition, 0);
        }
        
        var cameFrom = new Dictionary<StateTransition, StateTransition>();
        var gScore = new Dictionary<string, int> { { originHash, 0 } };

        while (openSet.TryDequeue(out var current, out var priority))
        {
            if (current.StageHash != originStageHash)
            {
                return ReconstructPathFrom(current).ToList();
            }

            var currentHash = current.DestinationHash;

            foreach (var neighbour in knownStates[currentHash].Neighbours(ref knownStates))
            {
                var neighbourHash = neighbour.SourceHash;
                
                var tentativeScore = gScore[current.SourceHash] + 1; // Checking wrong hash here?
                if (gScore.ContainsKey(neighbourHash) && tentativeScore >= gScore[neighbourHash]) continue;
                
                cameFrom[neighbour] = current;
                gScore[neighbourHash] = tentativeScore;
                if (openSet.UnorderedItems.All(tuple => tuple.Element.DestinationHash != neighbourHash && !knownStates.ContainsKey(tuple.Element.DestinationHash)))
                {
                    openSet.Enqueue(neighbour, tentativeScore);
                }
            }
        }
        
        return Enumerable.Empty<StateTransition>();
        
        IEnumerable<StateTransition> ReconstructPathFrom(StateTransition lastMove)
        {
            var path = new LinkedList<StateTransition>();
            path.AddFirst(lastMove);

            var previousMove = lastMove;

            while (cameFrom!.ContainsKey(previousMove))
            {
                previousMove = cameFrom[previousMove];
                path.AddFirst(previousMove);
            }
            
            return path;
        }
    }
}

public record StateTransition(Func<GameState, GameState> TransitionMove, string SourceHash, string DestinationHash, string StageHash);
