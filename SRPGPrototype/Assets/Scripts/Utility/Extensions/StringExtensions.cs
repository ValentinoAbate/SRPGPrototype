using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class StringExtensions
{
    public static int IndexOf(this string str, char search, int startPos, char[] escapes)
    {
        for (int i = startPos; i < str.Length; i++)
        {
            char c = str[i];
            if (escapes.Contains(c))
            {
                i++;
            }
            else if (c == search)
            {
                return i;
            }
        }
        return -1;
    }

    public static int IndexOfAny(this string str, char[] anyOf, int startPos, char[] escapes)
    {
        for (int i = startPos; i < str.Length; i++)
        {
            char c = str[i];
            if (escapes.Contains(c))
            {
                i++;
            }
            else if (anyOf.Contains(c))
            {
                return i;
            }
        }
        return -1;
    }

    public static string[] Split(this string str, char[] delims, char[] escapes)
    {
        var strl = new List<string>();
        var strb = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (escapes.Contains(c))
            {
                strb.Append(str[++i]);
            }
            else if (delims.Contains(c))
            {
                strl.Add(strb.ToString());
                strb.Clear();
            }
            else
            {
                strb.Append(c);
            }
        }
        if (strb.Length > 0) strl.Add(strb.ToString());
        return strl.ToArray();
    }
}
