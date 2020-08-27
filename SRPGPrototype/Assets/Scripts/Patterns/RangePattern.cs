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

    public List<Vector2Int> GetPositions(BattleGrid grid, Unit user)
    {
        switch (patternType)
        {
            case Type.Self:
                return new List<Vector2Int> { user.Pos };
            case Type.Adjacent:
                return user.Pos.Adjacent().ToList();
            case Type.AdjacentDiagonal:
                return user.Pos.AdjacentDiagonal().ToList();
            case Type.AdjacentBoth:
                return user.Pos.AdjacentDiagonal().Concat(user.Pos.Adjacent()).ToList();
            case Type.Ranged:
                return user.Pos.Adjacent(2).ToList();
            case Type.RangedDiagonal:
                return user.Pos.AdjacentDiagonal(2).ToList();
            case Type.Pattern:
                return pattern.OffsetsShifted(user.Pos).ToList(); ;
            case Type.Generated:
                return generator.Generate(grid, user);
            case Type.Horizontal:
                return new List<Vector2Int>() { user.Pos + Vector2Int.left, user.Pos + Vector2Int.right };
            case Type.Vertical:
                return new List<Vector2Int>() { user.Pos + Vector2Int.up, user.Pos + Vector2Int.down };
        }
        throw new System.Exception("Range Pattern Error: Invalid pattern type");
    }
}
