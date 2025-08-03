using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSelectUnit : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData) { }

    public override bool IsValidTarget(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null;
    }
}
