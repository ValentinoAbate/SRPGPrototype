using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierTargetPatternPierce : ModifierTargetPattern
{
    public override IEnumerable<Vector2Int> Modify(TargetPattern t, BattleGrid grid, Unit user, Vector2Int targetPos, RangePattern range)
    {
        bool Predicate(Vector2Int pos) =>  grid.IsLegal(pos) && !grid.IsEmpty(pos);
        bool isProjectile = range.patternType == RangePattern.Type.Generated && range.generator is RangePatternGeneratorDirectionalRaycast;
        Vector2Int Selector(Vector2Int pos)
        {
            if (isProjectile)
            {
                var pierceTarget = grid.Raycast(pos, user.Pos.DirectionTo(pos));
                return pierceTarget != null ? pierceTarget.Pos : pos;
            }
            else
            {
                return pos + user.Pos.DirectionTo(pos);
            }
        }
        foreach (var pos in UniqueWithSelector(t.Target(grid, user, targetPos), Selector, Predicate))
        {
            yield return pos;
        }
    }

    protected override bool AppliesToPattern(TargetPattern.Type t, TargetPatternGenerator generator)
    {
        return t != TargetPattern.Type.Self && t != TargetPattern.Type.Generated && t != TargetPattern.Type.Pattern;
    }
}
