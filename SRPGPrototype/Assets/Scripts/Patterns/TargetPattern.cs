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
        DirectionalPatternAndSelf,
        DirectionalPatternShift1
    }

    public Type patternType = Type.Simple;
    public Pattern pattern;
    public TargetPatternGenerator generator = null;

    public IEnumerable<Vector2Int> Target(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        if(patternType == Type.Self)
        {
            yield return user.Pos;
        }
        else if(patternType == Type.Simple)
        {
            yield return targetPos;
        }
        else if(patternType == Type.Pattern)
        {
            foreach (var pos in pattern.OffsetsShifted(targetPos))
                yield return pos;
        }
        else if(patternType == Type.Generated)
        {
            foreach (var pos in generator.Generate(grid, user, targetPos))
                yield return pos;
        }
        else if(patternType == Type.DirectionalPattern)
        {
            foreach (var pos in TargetDirectionalPattern(user, targetPos))
                yield return pos;
        }
        else if(patternType == Type.DirectionalPatternAndSelf)
        {
            foreach (var pos in TargetDirectionalPattern(user, targetPos))
                yield return pos;
            yield return user.Pos;
        }
        else if(patternType == Type.DirectionalPatternShift1)
        {
            foreach (var pos in TargetDirectionalPatternShiftedTowardsUser(user, targetPos, 1))
                yield return pos;
        }
    }

    private IEnumerable<Vector2Int> TargetDirectionalPattern(Unit user, Vector2Int targetPos)
    {
        Vector2Int direction = user.Pos.DirectionTo(targetPos);
        Vector2Int patternCenter = user.Pos + Vector2Int.right * (int)Vector2Int.Distance(user.Pos, targetPos) + Vector2Int.down * pattern.Center.y;
        return pattern.OffsetsShifted(patternCenter, false).Select((p) => p.Rotated(user.Pos, Vector2Int.right, direction));
    }

    private IEnumerable<Vector2Int> TargetDirectionalPatternShiftedTowardsUser(Unit user, Vector2Int targetPos, int shift)
    {
        Vector2Int direction = user.Pos.DirectionTo(targetPos);
        Vector2Int patternCenter = user.Pos + (Vector2Int.right * ((int)Vector2Int.Distance(user.Pos, targetPos) - shift)) + Vector2Int.down * pattern.Center.y;
        return pattern.OffsetsShifted(patternCenter, false).Select((p) => p.Rotated(user.Pos, Vector2Int.right, direction));
    }

    /// <summary>
    /// Returns the spaces that would hit the targetPos argument when targeted
    /// WARNING: Directional and Generated target patterns unsupported
    /// WARNING: Pattern type patterns only supported when symmetrical (unefined behavior)
    /// </summary>
    public IEnumerable<Vector2Int> ReverseTarget(BattleGrid grid, Vector2Int targetPos)
    {
        if (patternType == Type.Simple)
        {
            yield return targetPos;
        }
        else if (patternType == Type.Self)
        {
            foreach(var pos in grid.Dimensions.Enumerate())
            {
                yield return pos;
            } 
        }
        else if (patternType == Type.Pattern) // Only supported when symmetrical
        {
            foreach(var pos in pattern.OffsetsShifted(targetPos))
            {
                yield return pos;
            }
        }
        else if (patternType == Type.Generated) // May have to be per generator
        {
            Debug.LogError("Generated target patterns do not support reverse targeting");
        }
        else if (patternType == Type.DirectionalPattern) // May have to be in all range-allowed directions
        {
            Debug.LogError("Directional target patterns do not support reverse targeting");
        }
        else
        {
            Debug.LogError("Invalid target pattern type");
        }
    }
}
