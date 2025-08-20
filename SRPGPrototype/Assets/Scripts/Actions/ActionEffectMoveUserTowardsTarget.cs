using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveUserTowardsTarget : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !user.Movable)
            return;
        Move(grid, user, user, user.Pos.DirectionTo(target.Pos));
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !user.Movable)
            return false;
        return CanMove(grid, user, user, user.Pos.DirectionTo(target.Pos));
    }
}
