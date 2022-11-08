using System.Text.RegularExpressions;

namespace CCompiler.utils;

public static class StringUtil
{
    public static bool IsIdentPre(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }
    
    public static bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }
    
    public static bool IsIdentLetter(char c)
    {
        return IsIdentPre(c) || IsDigit(c);
    }
}