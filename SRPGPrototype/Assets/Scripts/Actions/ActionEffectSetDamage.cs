using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSetDamage : ActionEffect
{
    [SerializeField] private ActionEffectDamageSaved effect;
    [SerializeField] private int value;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        effect.damage = value;
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return true;
    }
}
