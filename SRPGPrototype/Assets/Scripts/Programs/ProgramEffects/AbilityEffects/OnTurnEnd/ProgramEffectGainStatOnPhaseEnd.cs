using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectGainStatOnPhaseEnd : ProgramEffectAddOnPhaseEndAbility
{
    public Stats.StatName stat;
    public UnitNumber number;
    protected override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;
    public override void Ability(BattleGrid grid, Unit unit)
    {
        unit.ModifyStat(grid, stat, number.Value(unit), unit);
    }
}
