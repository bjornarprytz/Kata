using System.Security.Cryptography;
using System.Text;

namespace Kata.BloomFilter;

public static class Hash
{
    public static byte[] Sha(string word)
    {
        using var sha = SHA256.Create();
        
        return sha.ComputeHash(Encoding.UTF8.GetBytes(word));
    }
}

