namespace Kata.WordChains;

public static class StringExtensions
{
    public static bool WordsDifferByOneLetter(string word1, string word2)
    {
        if (word1.Length != word2.Length)
            return false;

        return Enumerable.Range(0, word1.Length).Count(i => word1[i] != word2[i]) == 1;
    }
}