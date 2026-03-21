using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveUserTowardsTarget : ActionEffectMove
{
    [SerializeField] private bool scale = false;

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !user.Movable)
            return;
        Move(grid, user, user, user.Pos.DirectionTo(target.Pos), GetMoveDistance(user, target));
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !user.Movable)
            return false;
        return CanMove(grid, user, user, user.Pos.DirectionTo(target.Pos), GetMoveDistance(user, target));
    }

    protected int GetMoveDistance(Unit user, Unit target)
    {
        return scale ? user.Pos.ChebyshevDistance(target.Pos) : 1;
    }
}
