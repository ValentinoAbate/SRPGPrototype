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
    }

    public Type patternType = Type.Adjacent;
    public Pattern pattern;
    public RangePatternGenerator generator = null;

    public IEnumerable<Vector2Int> GetPositions(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        switch (patternType)
        {
            case Type.Self:
                return new Vector2Int[] { userPos };
            case Type.Adjacent:
                return userPos.Adjacent();
            case Type.AdjacentDiagonal:
                return userPos.AdjacentDiagonal();
            case Type.AdjacentBoth:
                return userPos.AdjacentBoth();
            case Type.Ranged:
                return userPos.Adjacent(2);
            case Type.RangedDiagonal:
                return userPos.AdjacentDiagonal(2);
            case Type.Pattern:
                return pattern.OffsetsShifted(userPos);
            case Type.Generated:
                return generator.Generate(grid, userPos, user);
            case Type.Horizontal:
                return new Vector2Int[] { userPos + Vector2Int.left, userPos + Vector2Int.right };
            case Type.Vertical:
                return new Vector2Int[] { userPos + Vector2Int.up, userPos + Vector2Int.down };
        }
        throw new System.Exception("Range Pattern Error: Invalid pattern type");
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
        switch (patternType)
        {
            case Type.Self:
                return 0;
            case Type.Adjacent:
                return 1;
            case Type.AdjacentDiagonal:
                return 2;
            case Type.AdjacentBoth:
                return 2;
            case Type.Ranged:
                return 2;
            case Type.RangedDiagonal:
                return 4;
            case Type.Pattern:
                return Mathf.Max(pattern.Dimensions.x, pattern.Dimensions.y);
            case Type.Generated:
                return generator.MaxDistance(grid);
            case Type.Horizontal:
                return 1;
            case Type.Vertical:
                return 1;
        }
        throw new System.Exception("Range Pattern Error: Invalid pattern type");
    }
}
