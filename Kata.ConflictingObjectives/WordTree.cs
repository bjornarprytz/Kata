using System.Collections.Generic;
using System.Linq;

namespace Kata.ConflictingObjectives;

public class WordTree
{
    private readonly LetterNode _root;

    public WordTree()
    {
        _root = new LetterNode();
    }
        
    public void Add(string word)
    {
        var current = word.Aggregate(_root, (current1, letter) => current1.AddChild(letter));

        current.EndOfWord();
    }

    public bool Contains(string word)
    {
        var current = _root;

        var result = word.All(letter => current!.TryGetChild(letter, out current)) 
                            && current.IsEndOfWord;

        return result;
    }
    private class LetterNode
    {
        private readonly Dictionary<char, LetterNode> _children = new();
        public bool IsEndOfWord { get; private set; }

        public LetterNode AddChild(char c)
        {
            if (!_children.ContainsKey(c))
            {
                _children.Add(c, new LetterNode());
                    
            }
                    
            return _children[c];
        }

        public bool TryGetChild(char letter, out LetterNode? child)
        {
            child = default;
                
            if (!_children.ContainsKey(letter))
                return false;

            child = _children[letter];

            return true;
        }

        public void EndOfWord()
        {
            IsEndOfWord = true;
        }
    }
}