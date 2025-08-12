using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectModifyDamage : ActionEffect
{
    [SerializeField] private ActionEffectDamageBasic effect;
    [SerializeField] private int modifier;

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        effect.damage += modifier;
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return true;
    }
}
