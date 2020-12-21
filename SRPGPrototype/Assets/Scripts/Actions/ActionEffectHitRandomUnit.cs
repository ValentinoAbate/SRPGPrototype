using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectHitRandomUnit : ActionEffect
{
    [SerializeField] private ActionEffect effectToApply;

    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        effectToApply.Initialize(grid, action, sub, user, targetPositions);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        var targets = grid.FindAll((u) => teams.Contains(u.UnitTeam));
        if (targets.Count <= 0)
            return;
        var newTarget = RandomUtils.RandomU.instance.Choice(targets);
        var newTargetData = new PositionData() { targetPos = newTarget.Pos, selectedPos = targetData.selectedPos };
        effectToApply.ApplyEffect(grid, action, sub, user, newTarget, newTargetData);
    }
}
