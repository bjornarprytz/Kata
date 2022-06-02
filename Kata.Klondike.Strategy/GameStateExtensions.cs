using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Kata.Klondike.Rules;
using Newtonsoft.Json;

namespace Kata.Klondike.Strategy;

public static class GameStateExtensions
{
    public static string ComputeHash(this GameState gameState)
    {
        var json = JsonConvert.SerializeObject(gameState with { FaceDownCards = ArraySegment<Card>.Empty });

        using var sha = SHA256.Create();
        
        return Encoding.ASCII.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(json)));
    }

    public static string ComputeStageHash(this GameState gameState)
    {
        var json = JsonConvert.SerializeObject(gameState.FaceDownCards.OrderBy(card => card, new Card.Comparer()));

        using var sha = SHA256.Create();
        
        return Encoding.ASCII.GetString(sha.ComputeHash(Encoding.UTF8.GetBytes(json)));
    }

    public static IEnumerable<StateTransition> GetPossibleTransitions(this GameState gameState)
    {
        var currentHash = gameState.ComputeHash();
        var stageHash = gameState.ComputeStageHash();

        var possibleTransitions = new List<StateTransition>();

        /*
         * TODO: There's potential to prune possibleActions.
         * No use trying a part of the gamestate that we _know_ hasn't changed.
         * E.g. don't re-try piles after calling PopStock()
         *
         * Maybe cache results, and set cache keys dirty when they change
         */
        
        
        var possibleActions = new List<Expression<Func<GameState, GameState>>>(); 

        possibleActions.Add(gs => gs.MoveCardFromDiscardToFoundation());
        possibleActions.Add(gs => gs.PopStock());
        
        for (var i = 0; i < gameState.Piles.Count; i++)
        {
            var sourceIndex = i;
            
            possibleActions.Add(gs => gs.MoveCardFromDiscardToTableauPile(gameState.Piles[sourceIndex]));
            possibleActions.Add(gs => gs.MoveCardFromPileToFoundation(gameState.Piles[sourceIndex]));
            
            for (var y = 0; y < gameState.Piles.Count; y++)
            {
                if (i == y) continue;
                var targetIndex = y;

                possibleActions.Add(gs => gs.MovePileToOtherPile(gs.Piles[sourceIndex], gs.Piles[targetIndex]));
            }
        }

        foreach (var transitionAction in possibleActions)
        {
            var newGameState = transitionAction.Compile().Invoke(gameState);

            if (newGameState.ComputeHash() != currentHash)
            {
                possibleTransitions.Add(new StateTransition(gameState, transitionAction));
            }
        }

        return possibleTransitions;
    }
}