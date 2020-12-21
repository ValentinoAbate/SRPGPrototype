using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;

public class ActionEffectGamble : ActionEffect
{
    [Range(0, 1)]
    [SerializeField] private float successChance = 0.5f;
    [SerializeField] private ActionEffect successEffect = null;
    [SerializeField] private ActionEffect failureEffect = null;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if((float)RandomU.instance.RandomDouble() < successChance)
        {
            successEffect.ApplyEffect(grid, action, sub, user, target, targetData);
        }
        else if (failureEffect != null)
        {
            failureEffect.ApplyEffect(grid, action, sub, user, target, targetData);
        }
    }
}
