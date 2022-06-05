using Kata.Klondike.Rules;

namespace Kata.Klondike.Strategy;

public interface IKlondikePlayer
{
    GameState TryToWin(GameState gameState);
}