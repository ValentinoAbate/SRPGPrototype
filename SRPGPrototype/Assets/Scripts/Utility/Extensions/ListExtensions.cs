using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
    public static void EnsureCapacity<T>(this List<T> list, int capacity)
    {
        if (list.Capacity >= capacity)
            return;
        list.Capacity = capacity;
    }
}
