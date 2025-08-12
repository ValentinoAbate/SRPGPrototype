using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSwap : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        grid.SwapAndSetWorldPos(user, target);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return user.Movable && target != null && target.Movable;
    }
}
