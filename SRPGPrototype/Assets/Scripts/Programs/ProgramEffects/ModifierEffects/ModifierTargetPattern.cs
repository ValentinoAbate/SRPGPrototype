using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierTargetPattern : ModifierAction
{
    public override bool AppliesTo(SubAction sub)
    {
        return base.AppliesTo(sub) && AppliesToPattern(sub.TargetType);
    }

    public abstract IEnumerable<Vector2Int> Modify(TargetPattern t, BattleGrid grid, Unit user, Vector2Int targetPos);

    protected abstract bool AppliesToPattern(TargetPattern.Type t);
}
