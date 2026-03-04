using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterNonHybridAction : ProgramEffectAddStatAfterActionAbility
{
    protected override bool AppliesToAction(Action action, int cost, Unit user, out int baseValue)
    {
        if(action.ActionType == Action.Type.Hybrid)
        {
            baseValue = 0;
            return false;
        }
        return base.AppliesToAction(action, cost, user, out baseValue);
    }
}
