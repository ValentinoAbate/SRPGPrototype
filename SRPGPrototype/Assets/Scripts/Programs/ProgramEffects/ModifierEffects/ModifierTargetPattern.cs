using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierTargetPattern : ModifierAction
{
    public override bool AppliesTo(SubAction sub)
    {
        return base.AppliesTo(sub) && AppliesToPattern(sub.TargetType, sub.TargetPatternGenerator);
    }

    public abstract IEnumerable<Vector2Int> Modify(TargetPattern t, BattleGrid grid, Unit user, Vector2Int targetPos, RangePattern range);

    protected abstract bool AppliesToPattern(TargetPattern.Type t, TargetPatternGenerator generator);
}
