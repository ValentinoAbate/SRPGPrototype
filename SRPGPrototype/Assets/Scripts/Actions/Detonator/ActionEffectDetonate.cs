using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDetonate : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        var detonatable = target.GetComponent<IDetonatable>();
        if (detonatable == null)
            return;
        detonatable.Detonate(grid, target, user);
    }
}
