using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageGrantFreeUseOnKill : ActionEffectDamageBasic
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || target.Dead)
            return;
        DealDamage(grid, action, sub, user, target, targetData);
        if (target.Dead)
        {
            action.GrantFreeUse();
        }
    }
}
