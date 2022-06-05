using Kata.Klondike.Rules;

namespace Kata.Klondike.Strategy;

public class ExhaustiveKlondikePlayer : IKlondikePlayer
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
    
    public static IEnumerable<StateTransition> FindTransitionsToNextStage(GameState currentGameState)
    {
        // Based on A-Star
        
        var originStageHash = currentGameState.ComputeStageHash();
        var originHash = currentGameState.ComputeHash();

        var knownStates = new Dictionary<string, GameState>{{originHash, currentGameState}};
        var openSet = new PriorityQueue<StateTransition, int>();
        var cameFrom = new Dictionary<StateTransition, StateTransition>();
        var gScore = new Dictionary<string, int> { { originHash, 0 } };

        foreach (var transition in currentGameState.Neighbours(ref knownStates))
        {
            openSet.Enqueue(transition, 0);
            gScore[transition.DestinationHash] = 1;
        }
        

        while (openSet.TryDequeue(out var current, out var priority))
        {
            if (current.StageHash != originStageHash)
            {
                return ReconstructPathFrom(current);
            }

            var currentState = knownStates[current.DestinationHash];

            foreach (var neighbourState in currentState.Neighbours(ref knownStates))
            {
                var neighbourHash = neighbourState.DestinationHash;
                
                var tentativeScore = gScore[neighbourState.SourceHash] + 1;
                if (gScore.ContainsKey(neighbourHash) && tentativeScore >= gScore[neighbourHash]) continue;
                
                cameFrom[neighbourState] = current;
                gScore[neighbourHash] = tentativeScore;
                if (openSet.UnorderedItems.All(tuple => tuple.Element.DestinationHash != neighbourHash))
                {
                    openSet.Enqueue(neighbourState, tentativeScore);
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
