using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kata.Anagrams;

public class AnagramsGrouper
{
    private readonly List<string> _words;
    
    private readonly Dictionary<string, HashSet<string>> _anagramGroups = new(); 

    public AnagramsGrouper(string input)
    {
        _words = input.Split('\n').ToList();
    }

    public void Process()
    {
        foreach (var word in _words)
        {
            var key = ComputeKey(word);

            if (!_anagramGroups.ContainsKey(key))
            {
                _anagramGroups[key] = new HashSet<string>();
            }
            
            _anagramGroups[key].Add(word);
        }

        foreach (var key in _anagramGroups.Keys.ToList().Where(key => _anagramGroups[key].Count == 1))
        {
            _anagramGroups.Remove(key);
        }
    }

    public string DumpStats()
    {
        var sb = new StringBuilder();

        sb.Append("Sets of anagrams: ");
        sb.AppendLine(_anagramGroups.Count.ToString());
        
        var longestAnagramGroup = _anagramGroups.MaxBy(pair => pair.Key.Length).Value;
        sb.AppendLine($"Longest anagram ({longestAnagramGroup.First().Length}):");
        sb.AppendLine(string.Join(' ', longestAnagramGroup));
        var largestAnagramSet = _anagramGroups.MaxBy(pair => pair.Value.Count).Value;
        sb.AppendLine($"Largest anagram set ({largestAnagramSet.Count}):");
        sb.AppendLine(string.Join(' ', largestAnagramSet));
        
        return sb.ToString();
    }
    
    public string DumpAnagrams()
    {
        return string.Join('\n', _anagramGroups.Values.Select(list => string.Join(' ', list)));
    }

    public IEnumerable<string> Anagrams(string word)
    {
        var key = ComputeKey(word);

        return _anagramGroups.ContainsKey(key)
            ? _anagramGroups[key]
            : Enumerable.Empty<string>();
    }

    private static string ComputeKey(string word)
    {
        return new string(word.OrderBy(c => c).ToArray());
    }
}
