using Kata.ConflictingObjectives;

namespace Kata.WordChains;

public class ExtendedWordTree : WordTree
{
    public IEnumerable<string> AllWordsOfLength(int length)
    {
        return WordsStartingWith("", Root, length);

        IEnumerable<string> WordsStartingWith(string start, LetterNode node, int lettersLeft)
        {
            if (lettersLeft == 0)
            {
                if (node.IsEndOfWord)
                    yield return start;
                else
                    yield break;
            }
            else
            {
                foreach (var (nextLetter, nextNode) in node.PossibleNextLetters())
                {
                    foreach (var word in WordsStartingWith(string.Concat(start, nextLetter), nextNode, lettersLeft - 1))
                    {
                        yield return word;
                    }
                }
            }
        }
    }
}