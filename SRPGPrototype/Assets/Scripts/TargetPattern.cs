using System.Collections;
using System.Collections.Generic;
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
            return new List<Vector2Int>(pattern.OffsetsShifted(targetPos - pattern.Center));
        return generator.Generate(grid, user, targetPos); 

    }
}
