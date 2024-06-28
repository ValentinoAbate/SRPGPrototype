using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;

public static class Vector2IntExtensions
{
    private const float sqrt2 = 1.4142135623746f;
    private const float negSqrt2 = -sqrt2;
    /// <summary>
    /// Rotates a point around another point another point.
    /// startDirection and goalDirection should either be Vector2Int.up, Vector2Int.down, Vector2Int.left, or Vector2Int.right
    /// </summary>
    public static Vector2Int Rotated(this Vector2Int point, Vector2Int center, Vector2Int startDirection, Vector2Int goalDirection)
    {
        // Already in the proper direction
        if (startDirection == goalDirection)
            return point;
        // Do basic vector rotation
        Vector2Int vec = point - center;
        float angle = Vector2.SignedAngle(startDirection, goalDirection) * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        Vector2 rotated = new Vector2(cos * vec.x - sin * vec.y, sin * vec.x + cos * vec.y);
        // Account for the distorted grid dimensions
        if(startDirection.sqrMagnitude != goalDirection.sqrMagnitude)
        {
            return center + Vector2Int.RoundToInt(rotated * (startDirection.sqrMagnitude == 1 ? sqrt2 : negSqrt2));
        }
        return center + Vector2Int.RoundToInt(rotated);
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

    public static IEnumerable<Vector2Int> AdjacentBoth(this Vector2Int pos)
    {
        return Adjacent(pos).Concat(AdjacentDiagonal(pos));
    }

    public static IEnumerable<Vector2Int> AdjacentBoth(this Vector2Int pos, int distance)
    {
        return Adjacent(pos, distance).Concat(AdjacentDiagonal(pos, distance));
    }

    public static IEnumerable<Vector2Int> Adjacent(this Vector2Int pos)
    {
        return new Vector2Int[]
        {
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.left,
            pos + Vector2Int.right,
        };
    }

    public static Vector2Int[] Adjacent(this Vector2Int pos, ref Vector2Int[] output)
    {
        output[0] = pos + Vector2Int.up;
        output[1] = pos + Vector2Int.down;
        output[2] = pos + Vector2Int.left;
        output[3] = pos + Vector2Int.right;
        return output;
    }

    public static void AddAdjacent(this Vector2Int pos, IList<Vector2Int> output)
    {
        output.Add(pos + Vector2Int.up);
        output.Add(pos + Vector2Int.down);
        output.Add(pos + Vector2Int.left);
        output.Add(pos + Vector2Int.right);
    }

    public static IEnumerable<Vector2Int> Adjacent(this Vector2Int pos, int distance)
    {
        return new Vector2Int[]
        {
            pos + (Vector2Int.up * distance),
            pos + (Vector2Int.down * distance),
            pos + (Vector2Int.left * distance),
            pos + (Vector2Int.right * distance),
        };
    }

    public static IEnumerable<Vector2Int> AdjacentDiagonal(this Vector2Int pos)
    {
        return new Vector2Int[]
        {
            pos + Vector2Int.up + Vector2Int.right,
            pos + Vector2Int.down + Vector2Int.right,
            pos + Vector2Int.down + Vector2Int.left,
            pos + Vector2Int.up + Vector2Int.left,
        };
    }

    public static void AddAdjacentDiagonal(this Vector2Int pos, IList<Vector2Int> output)
    {
        output.Add(pos + Vector2Int.up + Vector2Int.right);
        output.Add(pos + Vector2Int.down + Vector2Int.right);
        output.Add(pos + Vector2Int.down + Vector2Int.left);
        output.Add(pos + Vector2Int.up + Vector2Int.left);
    }

    public static IEnumerable<Vector2Int> AdjacentDiagonal(this Vector2Int pos, int distance)
    {
        return new Vector2Int[]
        {
            pos + ((Vector2Int.up + Vector2Int.right) * distance),
            pos + ((Vector2Int.down + Vector2Int.right) * distance),
            pos + ((Vector2Int.down + Vector2Int.left) * distance),
            pos + ((Vector2Int.up + Vector2Int.left) * distance),
        };
    }

    public static int GridDistance(this Vector2Int pos, Vector2Int other)
    {
        return System.Math.Abs(pos.x - other.x) + System.Math.Abs(pos.y - other.y);
    }
}
