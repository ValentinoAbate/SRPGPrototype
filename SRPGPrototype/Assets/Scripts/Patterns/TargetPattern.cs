using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions.VectorIntDimensionUtils;

[System.Serializable]
public class TargetPattern
{
    public enum Type
    { 
        Self,
        Simple,
        Pattern,
        Generated,
        DirectionalPattern,
        DirectionalPatternAndSelf
    }

    public Type patternType = Type.Simple;
    public Pattern pattern;
    public TargetPatternGenerator generator = null;

    public IEnumerable<Vector2Int> Target(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        return patternType switch
        {
            Type.Self => new Vector2Int[] { user.Pos },
            Type.Simple => new Vector2Int[] { targetPos },
            Type.Pattern => pattern.OffsetsShifted(targetPos),
            Type.Generated => generator.Generate(grid, user, targetPos),
            Type.DirectionalPattern => TargetDirectionalPattern(user, targetPos),
            Type.DirectionalPatternAndSelf => TargetDirectionalPattern(user, targetPos).Concat(new Vector2Int[] { user.Pos }),
            _ => System.Array.Empty<Vector2Int>(),
        };
    }

    private IEnumerable<Vector2Int> TargetDirectionalPattern(Unit user, Vector2Int targetPos)
    {
        Vector2Int direction = user.Pos.DirectionTo(targetPos);
        Vector2Int patternCenter = user.Pos + Vector2Int.right * (int)Vector2Int.Distance(user.Pos, targetPos) + Vector2Int.down * pattern.Center.y;
        return pattern.OffsetsShifted(patternCenter, false).Select((p) => p.Rotated(user.Pos, Vector2Int.right, direction));
    }

    /// <summary>
    /// Returns the spaces that would hit the targetPos argument when targeted
    /// WARNING: Directional and Generated target patterns unsupported (throws exception)
    /// WARNING: Pattern type patterns only supported when symmetrical (unefined behavior)
    /// </summary>
    public IEnumerable<Vector2Int> ReverseTarget(BattleGrid grid, Vector2Int targetPos)
    {
        if (patternType == Type.Simple)
            return new Vector2Int[] { targetPos };
        if (patternType == Type.Self)
            return grid.Dimensions.Enumerate();
        if (patternType == Type.Pattern) // Only supported when symmetrical
            return pattern.OffsetsShifted(targetPos);
        if (patternType == Type.Generated) // May have to be per generator
            throw new System.Exception("Generated target patterns do not support reverse targeting");
        if (patternType == Type.DirectionalPattern) // May have to be in all range-allowed directions
        {
            throw new System.Exception("Directional target patterns do not support reverse targeting");
        }
        throw new System.Exception("Invalid target pattern type");
    }
}
