using Extensions.VectorIntDimensionUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RangePattern
{
    public enum Type
    {
        Self,
        Adjacent,
        AdjacentDiagonal,
        AdjacentBoth,
        Ranged,
        RangedDiagonal,
        Pattern,
        Generated,
        Horizontal,
        Vertical,
        RangedBoth,
    }

    public Type patternType = Type.Adjacent;
    public Pattern pattern;
    public RangePatternGenerator generator = null;

    public IEnumerable<Vector2Int> GetPositions(BattleGrid grid, Vector2Int origin, Unit user)
    {
        return patternType switch
        {
            Type.Self => new Vector2Int[] { origin },
            Type.Adjacent => origin.Adjacent(),
            Type.AdjacentDiagonal => origin.AdjacentDiagonal(),
            Type.AdjacentBoth => origin.AdjacentBoth(),
            Type.Ranged => origin.Adjacent(2),
            Type.RangedDiagonal => origin.AdjacentDiagonal(2),
            Type.Pattern => pattern.OffsetsShifted(origin),
            Type.Generated => generator.Generate(grid, origin, user),
            Type.Horizontal => new Vector2Int[] { origin + Vector2Int.left, origin + Vector2Int.right },
            Type.Vertical => new Vector2Int[] { origin + Vector2Int.up, origin + Vector2Int.down },
            Type.RangedBoth => origin.AdjacentBoth(2),
            _ => throw new System.Exception("Range Pattern Error: Invalid pattern type"),
        };
    }

    public IEnumerable<Vector2Int> ReverseRange(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        if(patternType == Type.Generated)
        {
            return generator.ReverseGenerate(grid, targetPos, user);
        }
        return GetPositions(grid, targetPos, user);
    }

    public int MaxDistance(BattleGrid grid)
    {
        return patternType switch
        {
            Type.Self => 0,
            Type.Adjacent => 1,
            Type.AdjacentDiagonal => 2,
            Type.AdjacentBoth => 2,
            Type.Ranged => 2,
            Type.RangedDiagonal => 4,
            Type.RangedBoth => 4,
            Type.Pattern => Mathf.Max(pattern.Dimensions.x, pattern.Dimensions.y),
            Type.Generated => generator.MaxDistance(grid),
            Type.Horizontal => 1,
            Type.Vertical => 1,
            _ => throw new System.Exception("Range Pattern Error: Invalid pattern type"),
        };
    }
}
