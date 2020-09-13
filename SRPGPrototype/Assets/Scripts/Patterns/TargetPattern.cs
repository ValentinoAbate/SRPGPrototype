using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TargetPattern
{
    public enum Type
    { 
        Self,
        Simple,
        Pattern,
        Generated,
        DirectionalPattern
    }

    public Type patternType = Type.Simple;
    public Pattern pattern;
    public TargetPatternGenerator generator = null;

    public List<Vector2Int> Target(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        if(patternType == Type.Simple)
            return new List<Vector2Int> { targetPos };
        if (patternType == Type.Self)
            return new List<Vector2Int> { user.Pos };
        if (patternType == Type.Pattern)
            return new List<Vector2Int>(pattern.OffsetsShifted(targetPos));
        if(patternType == Type.Generated)
            return generator.Generate(grid, user, targetPos);
        if(patternType == Type.DirectionalPattern)
        {
            Vector2Int direction = user.Pos.DirectionTo(targetPos);
            Vector2Int patternCenter = user.Pos + Vector2Int.right * (int)Vector2Int.Distance(user.Pos, targetPos) + Vector2Int.down * pattern.Center.y;
            return pattern.OffsetsShifted(patternCenter, false).Select((p) => p.Rotated(user.Pos, Vector2Int.right, direction)).ToList();
        }
        throw new System.Exception("Invalid target pattern type");
    }
}
