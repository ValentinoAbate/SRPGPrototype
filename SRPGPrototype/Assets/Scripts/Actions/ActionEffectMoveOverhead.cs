using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveOverhead : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !target.Movable)
            return;
        var targetPos = user.Pos - (target.Pos - user.Pos);
        if (!grid.IsLegal(targetPos))
            return;
        if (grid.TryGet(targetPos, out var blocker) && !Move(grid, user, blocker, user.Pos.DirectionTo(blocker.Pos)))
            return;
        SetPosition(grid, user, target, targetPos);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !target.Movable)
            return false;
        var targetPos = user.Pos - (target.Pos - user.Pos);
        if (!grid.IsLegal(targetPos))
            return false;
        return !grid.TryGet(targetPos, out var blocker) || CanMove(grid, user, blocker, user.Pos.DirectionTo(blocker.Pos));
    }
}
