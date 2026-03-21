using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectModifyStatOnSummon : ProgramEffectAddOnSummonAbility
{
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;

    [SerializeField] private Stats.StatName stat;
    [SerializeField] private UnitNumber number;

    protected override void OnSummon(BattleGrid grid, Unit self, Unit summoned)
    {
        summoned.ModifyStat(grid, stat, number.Value(summoned), self);
    }
}
