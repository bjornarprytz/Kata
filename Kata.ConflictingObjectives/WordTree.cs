using System.Collections.Generic;
using System.Linq;

namespace Kata.ConflictingObjectives;

public class WordTree
{
    protected readonly LetterNode Root;

    public WordTree()
    {
        Root = new LetterNode();
    }
        
    public void Add(string word)
    {
        var current = word.Aggregate(Root, (current1, letter) => current1.AddChild(letter));

        current.EndOfWord();
    }

    public bool Contains(string word)
    {
        var current = Root;

        var result = word.All(letter => current!.TryGetChild(letter, out current)) 
                            && current.IsEndOfWord;

        return result;
    }

    protected class LetterNode
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

        public IReadOnlyDictionary<char, LetterNode> PossibleNextLetters()
        {
            return _children;
        }

        public void EndOfWord()
        {
            IsEndOfWord = true;
        }
    }
}