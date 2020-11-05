using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramEffectGainStatOnDestroy : ProgramEffectAddOnAfterSubActionAbility
{
    public Stats.StatName stat;
    public Unit.Team[] teams = new Unit.Team[1] { Unit.Team.Enemy };
    protected override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;
    public override void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        int number = targets.Where((t) => t.Dead && teams.Contains(t.UnitTeam)).Count();
        if (number > 0)
            user.ModifyStat(grid, stat, number, user);
    }
}
