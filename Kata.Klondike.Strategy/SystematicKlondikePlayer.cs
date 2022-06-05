using Kata.Klondike.Rules;

namespace Kata.Klondike.Strategy;

public class SystematicKlondikePlayer : IKlondikePlayer
{
    public GameState TryToWin(GameState gameState)
    {
        return gameState;
    }
}