using System.Diagnostics;
using Kata.WordChains;

const string wordListPath = @"..\..\..\..\wordlist.txt";

var wordChain = new WordChain();

var words = File.ReadAllText(wordListPath).Split('\n');

wordChain.AddWords(words);

var pairs = new (string from, string to)[]
{
    ("ruby", "code"),
    ("gold", "lead"),
    ("bridge", "bottom"),
};

foreach (var (from, to) in pairs)
{
    Console.WriteLine(
        wordChain.TryWordChainGraphSearch(from, to, out var chain)
            ? string.Join(", ", chain)
            : $"No chain between {from} and {to}"
        );
}
