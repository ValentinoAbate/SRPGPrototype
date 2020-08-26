using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2Int DirectionTo(this Vector2Int from, Vector2Int to)
    {
        var direction = to - from;
        direction.Clamp(-Vector2Int.one, Vector2Int.one);
        return direction;
    }

    public static Vector2Int DirectionFrom(this Vector2Int to, Vector2Int from)
    {
        var direction = to - from;
        direction.Clamp(-Vector2Int.one, Vector2Int.one);
        return direction;
    }

    public static IEnumerable<Vector2Int> Adjacent(this Vector2Int pos)
    {
        return new List<Vector2Int>()
        {
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.left,
            pos + Vector2Int.right,
        };
    }

    public static IEnumerable<Vector2Int> AdjacentDiagonal(this Vector2Int pos)
    {
        return new List<Vector2Int>()
        {
            pos + Vector2Int.up + Vector2Int.right,
            pos + Vector2Int.down + Vector2Int.right,
            pos + Vector2Int.down + Vector2Int.left,
            pos + Vector2Int.up + Vector2Int.left,
        };
    }
}
