﻿using System.Collections.Generic;

namespace Pathfinding.Utils
{
    public static class DictExtensions
    {
        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }
    }
}