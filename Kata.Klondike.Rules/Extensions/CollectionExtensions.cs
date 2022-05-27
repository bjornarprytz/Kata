namespace Kata.Klondike.Rules;

public static class CollectionExtensions
{
    private static readonly Random Random = new ();

    private static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        var array = collection.ToArray();

        var n = array.Length;
        for (var i = 0; i < (n - 1); i++)
        {
            var r = i + Random.Next(n - i);

            (array[r], array[i]) = (array[i], array[r]);
        }

        return array;
    }

    public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> collection)
    {
        var queue = new Queue<T>(collection.Shuffle());

        while (queue.TryDequeue(out var next))
        {
            yield return next;
        }
    }

    public static int IndexOf<T>(this IEnumerable<T> collection, T value)
    {
        foreach (var (item, index) in collection.Select((pile, i) => (pile, i)))
        {
            if (item!.Equals(value))
            {
                return index;
            }
        }

        return -1;
    }

    public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}