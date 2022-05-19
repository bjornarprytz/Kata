using System.Text.RegularExpressions;
using Microsoft.Win32.SafeHandles;

namespace Kata.CountingCodeLines;

public static class LineCounter
{
    public static int Count(string code)
    {
        var isInBlockComment = false;

        return code
            .Split("\r\n")
            .Select(line => line.ParseLineForCode(ref isInBlockComment))
            .Count(containsCode => containsCode is true);
    }
}

public static class StringExtensions
{
    public static bool ParseLineForCode(this string line, ref bool isInBlockComment)
    {
        var lineContainsCode = false;
        
        for (;line.HasCharacters(); line = line[1..])
        {
            if (isInBlockComment && !TryEndBlockComment(ref line))
            {
                break;
            }
                
            isInBlockComment = false;
            line = line.Trim();

            if (line.IsEmpty())
            {
                break;
            }

            if (CheckForLineComment(line))
            {
                break;
            }

            if (CheckForString(ref line))
            {
                lineContainsCode = true;
                continue;
            }

            if (CheckForBlockComment(ref line))
            {
                isInBlockComment = true;
                continue;
            }
            
            lineContainsCode = true;
        }

        return lineContainsCode;
        
        bool CheckForLineComment(string code)
        {
            return code.StartsWith("//");
        }

        static bool CheckForBlockComment(ref string code)
        {
            if (!code.StartsWith("/*")) 
                return false;
            
            code = code[2..];
            return true;
        }

        static bool CheckForString(ref string code)
        {
            if (!code.StartsWith('"'))
                return false;
            
            var nextQuoteIndex = code[1..].IndexOf('"') + 1;
            
            code = nextQuoteIndex == 0 
                ? string.Empty 
                : code[nextQuoteIndex..];

            return true;
        }

        static bool TryEndBlockComment(ref string code)
        {
            var blockEnd = code.IndexOf("*/", StringComparison.Ordinal);

            if (blockEnd == -1)
                return false;

            blockEnd += 2; // Skip "*/"

            code = code[blockEnd..];

            return true;
        }
    }
    
    private static bool IsEmpty(this string s)
    {
        return string.IsNullOrWhiteSpace(s);
    }

    private static bool HasCharacters(this string s)
    {
        return !s.IsEmpty();
    }
}