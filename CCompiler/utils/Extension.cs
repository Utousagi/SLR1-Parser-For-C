 using System.Collections;
 using System.Reflection;

namespace CCompiler.utils;

// 拓展方法
public static class Extension
{
    public static MCharEnumerator GetMEnumerator(this string str)
    {
        return new MCharEnumerator(str);
    }

    public static void Add<TK, TV>(this Dictionary<TK, TV> dict, IEnumerable<TK> keys, TV value) where TK : notnull
    {
        foreach (var key in keys.Where(key => !dict.ContainsKey(key)))
        {
            dict.Add(key, value);
        }
    }
}

