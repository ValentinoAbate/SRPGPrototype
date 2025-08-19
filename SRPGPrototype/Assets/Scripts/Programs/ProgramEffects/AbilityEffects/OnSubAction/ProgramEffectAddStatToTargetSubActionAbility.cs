using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramEffectAddStatToTargetSubActionAbility : ProgramEffectAddSubActionAbility
{
    public Stats.StatName stat;
    public ActionNumber number;
    protected override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;

    [SerializeField] private Unit.Team[] affectedTeams = new Unit.Team[] { Unit.Team.Enemy };
    public override void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        foreach(var target in targets)
        {
            if (!affectedTeams.Contains(target.UnitTeam))
                continue;
            int value = number.ActionValue(grid, action, target, targets.Count);
            target.ModifyStat(grid, stat, value, user);
        }
    }
}
