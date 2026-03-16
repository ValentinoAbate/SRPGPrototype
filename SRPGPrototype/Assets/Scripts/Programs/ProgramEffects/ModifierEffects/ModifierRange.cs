using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierRange : ModifierAction
{
    public override bool AppliesTo(SubAction sub)
    {
        return base.AppliesTo(sub) && AppliesToPattern(sub.RangeType, sub.RangeGenerator);
    }

    public abstract IEnumerable<Vector2Int> Modify(RangePattern r, BattleGrid grid, Unit user, Vector2Int origin);

    protected abstract bool AppliesToPattern(RangePattern.Type t, RangePatternGenerator generator);
}
