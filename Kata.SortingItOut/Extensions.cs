namespace Kata.SortingItOut;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> range, Random rng)
    {
        var elements = range.ToArray();

        for (var i = elements.Length - 1; i >= 0; i--)
        {
            var swapIndex = rng.Next(i + 1);

            (elements[swapIndex], elements[i]) = (elements[i], elements[swapIndex]);
        }

        return elements;
    }
} 