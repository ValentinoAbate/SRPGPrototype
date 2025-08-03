using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMultiplyDamage : ActionEffect
{
    [SerializeField] private ActionEffectDamageBasic effect;
    [SerializeField] private int multiplier;

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        effect.damage *= multiplier;
    }

    public override bool IsValidTarget(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return true;
    }
}
