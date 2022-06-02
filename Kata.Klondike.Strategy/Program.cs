// See https://aka.ms/new-console-template for more information

using Kata.Klondike.Rules;
using Kata.Klondike.Strategy;

Console.WriteLine("Hello, World!");

var score = 0;
var totalGames = 1000;

foreach (var i in Enumerable.Range(0, totalGames))
{
    var gameState = GameAction.Init();

    var player = new KlondikePlayer();

    var result = player.TryToWin(gameState);

    if (result.Victorious)
    {
        score++;
        Console.WriteLine($"Won! ({i})");
    }

}

Console.WriteLine($"Won {score} / {totalGames} games");