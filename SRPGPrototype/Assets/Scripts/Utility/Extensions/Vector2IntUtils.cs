using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntUtils
{
    public const char separator = '_';
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
        foreach(var adjPos in Adjacent(pos))
        {
            yield return adjPos;
        }
        foreach (var adjPos in AdjacentDiagonal(pos))
        {
            yield return adjPos;
        }
    }

    public static IEnumerable<Vector2Int> AdjacentBoth(this Vector2Int pos, int distance)
    {
        foreach (var adjPos in Adjacent(pos, distance))
        {
            yield return adjPos;
        }
        foreach (var adjPos in AdjacentDiagonal(pos, distance))
        {
            yield return adjPos;
        }
    }

    public static IEnumerable<Vector2Int> Adjacent(this Vector2Int pos)
    {
        yield return pos + Vector2Int.up;
        yield return pos + Vector2Int.down;
        yield return pos + Vector2Int.left;
        yield return pos + Vector2Int.right;
    }

    public static IEnumerable<Vector2Int> Adjacent(this Vector2Int pos, int distance)
    {
        yield return pos + (Vector2Int.up * distance);
        yield return pos + (Vector2Int.down * distance);
        yield return pos + (Vector2Int.left * distance);
        yield return pos + (Vector2Int.right * distance);
    }

    public static IEnumerable<Vector2Int> AdjacentDiagonal(this Vector2Int pos)
    {
        yield return pos + Vector2Int.up + Vector2Int.right;
        yield return pos + Vector2Int.down + Vector2Int.right;
        yield return pos + Vector2Int.down + Vector2Int.left;
        yield return pos + Vector2Int.up + Vector2Int.left;
    }

    public static IEnumerable<Vector2Int> AdjacentDiagonal(this Vector2Int pos, int distance)
    {
        yield return pos + ((Vector2Int.up + Vector2Int.right) * distance);
        yield return pos + ((Vector2Int.down + Vector2Int.right) * distance);
        yield return pos + ((Vector2Int.down + Vector2Int.left) * distance);
        yield return pos + ((Vector2Int.up + Vector2Int.left) * distance);
    }

    public static int GridDistance(this Vector2Int pos, Vector2Int other)
    {
        return System.Math.Abs(pos.x - other.x) + System.Math.Abs(pos.y - other.y);
    }

    public static string Save(this Vector2Int pos)
    {
        return $"{pos.x}{separator}{pos.y}";
    }

    public static Vector2Int FromString(string data)
    {
        var args = data.Split(separator);
        if (args.Length < 2 || !int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y))
            return Vector2Int.zero;
        return new Vector2Int(x, y);
    }
}
