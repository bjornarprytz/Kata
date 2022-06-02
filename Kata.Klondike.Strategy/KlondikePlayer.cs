using System.Linq.Expressions;
using Kata.Klondike.Rules;
using Kata.WordChains;

namespace Kata.Klondike.Strategy;

public class KlondikePlayer
{
    
    

    private class Stage
    {
        private readonly GameState _entryState;
        private readonly HashSet<string> _knownStates = new ();

        public Stage(GameState entryState)
        {
            _entryState = entryState;
        }
        
        public IEnumerable<StateTransition> Process()
        {
            // search through all states
            
            return Enumerable.Empty<StateTransition>();
        }
        
        public void Analyse()
        {
            
        }
    }

    private readonly Dictionary<string, Stage> _knownStages = new();

    public IEnumerable<StateTransition> FindTransitionsToNextStage(GameState currentGameState)
    {
        var transitions = new List<StateTransition>();


        return transitions;
    }
}

public record StateTransition(GameState State, Expression<Func<GameState, GameState>> ActionExpression) : IGraphNode<StateTransition>
{
    private List<StateTransition>? _neighbours = null;
    public IEnumerable<StateTransition> Neighbours => _neighbours ??= State.GetPossibleTransitions().ToList();
}