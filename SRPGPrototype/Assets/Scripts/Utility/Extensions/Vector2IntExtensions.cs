using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Vector2IntExtensions
{
    /// <summary>
    /// Rotates a point around another point another point.
    /// startDirection and goalDirection should either be Vector2Int.up, Vector2Int.down, Vector2Int.left, or Vector2Int.right
    /// </summary>
    public static Vector2Int Rotated(this Vector2Int point, Vector2Int center, Vector2Int startDirection, Vector2Int goalDirection)
    {
        // Already in the proper direction
        if (startDirection == goalDirection)
            return point;
        Vector2Int difference = point - center;
        Vector2Int sumDirection = startDirection + goalDirection;
        // Directions are colinear
        if (sumDirection == Vector2Int.zero)
            return center - difference;
        // Directions are perpendicular
        return center + PointwiseProduct(difference.AxesSwapped(), sumDirection);
    }
    /// <summary>
    /// Return the Pointwise product of two Vector2Int (considering them as 2D int vectors).
    /// Returns new Vector2Int(p1.row * p2.row, p1.col * p2.col);
    /// </summary>
    public static Vector2Int PointwiseProduct(this Vector2Int p1, Vector2Int p2)
    {
        return new Vector2Int(p1.x * p2.x, p1.y * p2.y);
    }

    public static Vector2Int AxesSwapped(this Vector2Int point)
    {
        return new Vector2Int(point.y, point.x);
    }

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

    public static IEnumerable<Vector2Int> Adjacent(this Vector2Int pos, int distance)
    {
        return new List<Vector2Int>()
        {
            pos + (Vector2Int.up * distance),
            pos + (Vector2Int.down * distance),
            pos + (Vector2Int.left * distance),
            pos + (Vector2Int.right * distance),
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

    public static IEnumerable<Vector2Int> AdjacentDiagonal(this Vector2Int pos, int distance)
    {
        return new List<Vector2Int>()
        {
            pos + ((Vector2Int.up + Vector2Int.right) * distance),
            pos + ((Vector2Int.down + Vector2Int.right) * distance),
            pos + ((Vector2Int.down + Vector2Int.left) * distance),
            pos + ((Vector2Int.up + Vector2Int.left) * distance),
        };
    }
}
