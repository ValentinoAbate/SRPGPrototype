using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoolUtils
{
    private const string one = "1";
    private const string zero = "0";

    public static string ToStringInt(bool b)
    {
        return b ? one : zero;
    }

    public static bool FromStringInt(string s)
    {
        return int.TryParse(s, out int i) && i == 1;
    }
}
