using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionConditionHasPassive : ActionCondition
{
    [SerializeField] private Unit.PassiveEffect effect;
    [SerializeField] private string failureMessage;
    public override bool CanUse(BattleGrid grid, Unit user, Action action, out string failMessage)
    {
        if (!user.HasEffect(effect))
        {
            failMessage = failureMessage;
            return false;
        }
        failMessage = null;
        return true;
    }
}
