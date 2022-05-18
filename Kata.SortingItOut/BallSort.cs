namespace Kata.SortingItOut;

public interface IRack60
{
    void Add(int number);
    IEnumerable<int> Balls { get; }
}

public class BitRack60 : IRack60
{
    private int _lowest = 60;
    private int _highest = -1;
    private int _zeroToFiftyNine = 0;

    public IEnumerable<int> Balls => Enumerate();

    public void Add(int number)
    {
        if (number is < 0 or > 59)
            throw new ArgumentException("This rack only accepts numbers in the range [0-59]");

        _zeroToFiftyNine |= (1 << number);

        _lowest = Math.Min(_lowest, number);
        _highest = Math.Max(_highest, number);
    }

    private IEnumerable<int> Enumerate()
    {
        if (_lowest == -1)
            yield break;

        yield return _lowest;

        for (var i = _lowest+1; i <= _highest; i++)
        {
            if ((1 << i & _zeroToFiftyNine) != 0)
            {
                yield return i;
            }
        }
    }
}

public class NaiveRack60 : IRack60
{
    private readonly List<int> _numbers = new();
    public void Add(int number)
    {
        if (!_numbers.Contains(number))
            _numbers.Add(number);
    }

    public IEnumerable<int> Balls => _numbers.OrderBy(i => i);
}

public class ArrayRack60 : IRack60
{
    private readonly bool[] _numbers = new bool[60];
    public void Add(int number)
    {
        _numbers[number] = true;
    }

    public IEnumerable<int> Balls => _numbers.Select((b, i) => b ? i : -1).Where(i => i != -1);
}