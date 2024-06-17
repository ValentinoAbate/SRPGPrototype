using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterActionAbility : ProgramEffectAddOnAfterActionAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private ActionNumber number;
    protected override void Ability(BattleGrid grid, Action action, Unit user, int cost)
    {
        if (!AppliesToAction(action, cost, user, out int value))
            return;
        user.ModifyStat(grid, stat, value, user);
    }

    protected virtual bool AppliesToAction(Action action, int cost, Unit user, out int baseValue)
    {
        baseValue = number.BaseValue(action, user);
        return baseValue != 0;
    }
}
