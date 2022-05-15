
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata.BloomFilter;

public class BloomFilter
{
    private readonly byte[] _filter;
    private readonly Func<string, byte[]> _hashFunc;
    private readonly int _hashCount;
    public BloomFilter(int size, int hashCount, Func<string, byte[]> hashFunc)
    {
        _hashCount = hashCount;
        _hashFunc = hashFunc;
        _filter = new byte[size];
    }

    public void Add(string word)
    {
        foreach(var (byteAddress, bitInByte) in GetAddress(word))
        {
            _filter[byteAddress] = (byte) (_filter[byteAddress] | bitInByte);
        }
    }

    public bool Check(string word)
    {
        var wordInDictionary = GetAddress(word).All(address =>
        {
            var (byteAddress, bitMask) = address;
            return (_filter[byteAddress] & bitMask) != 0;
        });
        return wordInDictionary;
    }

    private IEnumerable<(int, byte)> GetAddress(string word)
    {
        var addresses = new (int, byte)[_hashCount];
        
        var hash = _hashFunc(word);
        var address = Math.Abs(BitConverter.ToInt32(hash));
        var byteAddress = address % _filter.Length;
        var bitMask = (byte)(1 << (address % 8));

        for (var i = 0; i < _hashCount; i++)
        {
            addresses[i] = (byteAddress, bitMask);
        }

        return addresses;
    }
}

public static class BloomFilterExtensions
{
    public static void CheckWord(this BloomFilter filter, string word)
    {
        Console.WriteLine(filter.Check(word) 
            ? $"Hit on [{word}] :)" 
            : $"Miss on [{word}] :(");
    }
}