using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterActionCost : ProgramEffectAddStatAfterActionAbilityTurnLimit
{
    [SerializeField] private int costThreshold;

    protected override bool AppliesToAction(Action action, int cost, Unit user, out int baseValue)
    {
        baseValue = 0;
        return cost >= costThreshold && base.AppliesToAction(action, cost, user, out baseValue);
    }
}
