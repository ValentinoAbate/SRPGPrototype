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
        var blocker = grid.Get(targetPos);
        if (blocker != null && !Move(grid, user, blocker, user.Pos.DirectionTo(blocker.Pos)))
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
        var blocker = grid.Get(targetPos);
        return blocker == null || CanMove(grid, user, blocker, user.Pos.DirectionTo(blocker.Pos));
    }
}
