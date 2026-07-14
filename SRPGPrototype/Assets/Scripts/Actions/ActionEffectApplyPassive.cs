using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectApplyPassive : ActionEffect
{
    [SerializeField] private Unit.PassiveEffect effect;

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        target.AddPassiveEffect(effect);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return true;
    }
}
