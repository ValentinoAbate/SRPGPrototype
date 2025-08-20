using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSwap : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        Swap(grid, user, target, user);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return user.Movable && target != null && target.Movable;
    }
}
