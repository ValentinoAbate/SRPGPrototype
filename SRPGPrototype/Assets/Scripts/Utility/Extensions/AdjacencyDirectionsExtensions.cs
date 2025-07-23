using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AdjacencyDirectionsExtensions
{
    public static IEnumerable<Vector2Int> GetDirectionVectors(this AdjacencyDirections directions)
    {
        if (directions.HasFlag(AdjacencyDirections.Horizontal))
        {
            yield return Vector2Int.up;
            yield return Vector2Int.down;
            yield return Vector2Int.left;
            yield return Vector2Int.right;
        }
        if (directions.HasFlag(AdjacencyDirections.Diagonal))
        {
            yield return Vector2Int.up + Vector2Int.right;
            yield return Vector2Int.down + Vector2Int.right;
            yield return Vector2Int.down + Vector2Int.left;
            yield return Vector2Int.up + Vector2Int.left;
        }
    }
}
