namespace Kata.Anagrams;

public static class Extensions
{
    public static bool None<T>(this IEnumerable<T> items)
    {
        return !items.Any();
    }
}