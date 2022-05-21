namespace Kata.ADiversion;

public class NDigit1Bits
{
    public static int NumbersWithoutAdjacent1Bits(int nBits)
    {
        return nBits switch
        {
            < 1 => throw new ArgumentException($"Only valid for positive integers", nameof(nBits)),
            1 => 2,
            _ => nBits - 1 + NumbersWithoutAdjacent1Bits(nBits - 1)
        };
    }
}