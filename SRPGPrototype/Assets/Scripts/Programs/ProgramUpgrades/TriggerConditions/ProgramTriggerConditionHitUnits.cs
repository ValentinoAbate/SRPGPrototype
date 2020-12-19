using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerConditionHitUnits : ProgramTriggerConditionResetTrigger
{
    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };

    protected override string ProgressConditionText => "Hit " + string.Join("/", teams) + " units ";

    protected override int ProgressChange(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        return targets.Where((u) => teams.Contains(u.UnitTeam)).Count();
    }
}
