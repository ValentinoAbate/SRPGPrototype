using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSwap : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        grid.SwapAndSetWorldPos(user, target);
    }
}
