using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramEffectAddStatToAdjacentSubActionAbility : ProgramEffectAddSubActionAbility
{
    public Stats.StatName stat;
    public ActionNumber number;
    protected override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;
    [SerializeField] private Unit.Team[] affectedTeams = new Unit.Team[] { Unit.Team.Enemy };

    public override void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        foreach(var pos in user.Pos.Adjacent())
        {
            if (!grid.IsLegal(pos))
                continue;
            var target = grid.Get(pos);
            if (target == null || !affectedTeams.Contains(target.UnitTeam))
                continue;
            int value = number.ActionValue(grid, action, target, targets.Count);
            target.ModifyStat(grid, stat, value, user);
        }
    }
}
