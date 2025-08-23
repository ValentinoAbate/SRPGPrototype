using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatToOtherOnRepositionOther : ProgramEffectAddOnRepositionOtherAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private UnitNumber number;
    [SerializeField] private List<Unit.Team> targetTeams;
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;

    public override void Ability(BattleGrid grid, Unit source, Unit target)
    {
        if (targetTeams.Count > 0 && !targetTeams.Contains(target.UnitTeam))
            return;
        target.ModifyStat(grid, stat, number.Value(target), source);
    }
}
