using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierTargetPatternMirror : ModifierTargetPattern
{
    public override IEnumerable<Vector2Int> Modify(TargetPattern t, BattleGrid grid, Unit user, Vector2Int targetPos, RangePattern range)
    {
        Vector2Int Selector(Vector2Int pos) => user.Pos - (pos - user.Pos);
        foreach (var pos in Vector2IntUtils.UniqueWithSelector(t.Target(grid, user, targetPos), Selector))
        {
            yield return pos;
        }
    }

    protected override bool AppliesToPattern(TargetPattern.Type t, TargetPatternGenerator generator)
    {
        return t != TargetPattern.Type.Self;
    }
}
