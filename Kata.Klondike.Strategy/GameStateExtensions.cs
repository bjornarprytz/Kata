using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Kata.Klondike.Rules;
using Newtonsoft.Json;

namespace Kata.Klondike.Strategy;

public static class GameStateExtensions
{
    private static readonly Card.Comparer CardComparer = new();
    
    public static IEnumerable<StateTransition> Neighbours(this GameState gameState, ref Dictionary<string, GameState> knownGameStates) => gameState.GetPossibleTransitions(ref knownGameStates);
    
    public static string ComputeHash(this GameState gameState)
    {
        var json = JsonConvert.SerializeObject(gameState with { FaceDownCards = ArraySegment<Card>.Empty });
        
        using var sha = SHA256.Create();
        
        return Encoding.ASCII.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(json)));
    }

    public static string ComputeStageHash(this GameState gameState)
    {
        var json = JsonConvert.SerializeObject(gameState.FaceDownCards.OrderBy(card => card, CardComparer));

        using var sha = SHA256.Create();
        
        return Encoding.ASCII.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(json)));
    }

    private static IEnumerable<StateTransition> GetPossibleTransitions(this GameState gameState, ref Dictionary<string, GameState> knownGameStates)
    {
        var currentHash = gameState.ComputeHash();

        var transitiveActions = new List<StateTransition>();

        foreach (var transitionAction in CreatePossibleActionList(gameState.Piles.Count))
        {
            var newGameState = transitionAction(gameState);

            var newHash = newGameState.ComputeHash();

            if (newHash == currentHash) continue;
            
            transitiveActions.Add(new StateTransition(transitionAction, currentHash, newHash, newGameState.ComputeStageHash()));
            
            if (!knownGameStates.ContainsKey(newHash))
            {
                knownGameStates.Add(newHash, newGameState);
            }
        }

        return transitiveActions;
    }

    private static IReadOnlyList<Func<GameState, GameState>> PossibleActions { get; } = CreatePossibleActionList();
    private static IReadOnlyList<Func<GameState, GameState>> CreatePossibleActionList(int pileCount=7)
    {
        var possibleActions = new List<Func<GameState, GameState>>(); 

        possibleActions.Add(gs => gs.MoveCardFromDiscardToFoundation());
        possibleActions.Add(gs => gs.PopStock());
        
        for (var i = 0; i < pileCount; i++)
        {
            var sourceIndex = i;
            
            possibleActions.Add(gs => gs.MoveCardFromDiscardToTableauPile(gs.Piles[sourceIndex]));
            possibleActions.Add(gs => gs.MoveCardFromPileToFoundation(gs.Piles[sourceIndex]));
            
            for (var y = 0; y < pileCount; y++)
            {
                if (i == y) continue;
                var targetIndex = y;

                possibleActions.Add(gs => gs.MovePileToOtherPile(gs.Piles[sourceIndex], gs.Piles[targetIndex]));
            }
        }

        return possibleActions;
    }
}