using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectGrantFreeUseOnKill : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        if (target.Dead)
        {
            action.GrantFreeUse();
        }
    }
}
