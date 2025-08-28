using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.VectorIntDimensionUtils
{
    public static class Vector2IntDimensionUtils
    {
        public static bool IsBoundedBy(this Vector2Int vec, Vector2Int bounds)
        {
            return vec.x >= 0 && vec.y >= 0 && vec.x < bounds.x && vec.y < bounds.y;
        }

        public static bool IsOnEdge(this Vector2Int vec, Vector2Int dimensions)
        {
            return vec.x == 0 || vec.y == 0 || vec.x == dimensions.x - 1 || vec.y == dimensions.y - 1;
        }

        public static IEnumerable<Vector2Int> Enumerate(this Vector2Int dimensions)
        {
            for (int x = 0; x < dimensions.x; ++x)
                for (int y = 0; y < dimensions.y; ++y)
                    yield return new Vector2Int(x, y);
        }
    }
}