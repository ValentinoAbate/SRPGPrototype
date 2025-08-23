using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnProgramDestroyed : ProgramEffectAddOnProgramDestroyedAbility
{
    public Stats.StatName stat;
    public UnitNumber number;
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;

    protected override void Ability(Program p, BattleGrid grid, Unit user)
    {
        if (grid == null || user == null)
            return;
        user.ModifyStat(grid, stat, number.Value(user), user);
    }

}
