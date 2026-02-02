using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerConditionHitUnitsAtDistance : ProgramTriggerConditionResetTrigger
{
    [SerializeField] private int threshold = 4;
    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };

    protected override string ProgressConditionText => $"Hit {string.Join("/", teams)} units from at least {threshold} rows/cols away";

    protected override int ProgressChange(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        int progressGained = 0;
        foreach(var target in targets)
        {
            if (!teams.Contains(target.UnitTeam))
                continue;
            int distance = Math.Max(Math.Abs(user.Pos.x - target.Pos.x), Math.Abs(user.Pos.y - target.Pos.y));
            if(distance >= threshold)
                ++progressGained;
        }
        return progressGained;
    }
}
