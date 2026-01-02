using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectModifyUserStatOnKill : ActionEffectModifyStat
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !target.Dead)
            return;
        base.ApplyEffect(grid, action, sub, user, user, targetData);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null;
    }
}
