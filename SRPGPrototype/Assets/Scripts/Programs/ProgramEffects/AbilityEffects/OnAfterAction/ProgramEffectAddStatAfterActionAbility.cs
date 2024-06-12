using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterActionAbility : ProgramEffectAddOnAfterActionAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private ActionNumber number;
    protected override void Ability(BattleGrid grid, Action action, Unit user)
    {
        int value = number.BaseValue(action, user);
        user.ModifyStat(grid, stat, value, user);
    }

    protected bool AppliesToAction(Action action, Unit user)
    {
        return number.BaseValue(action, user) != 0;
    }
}
