using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectMoveDirection : ActionEffectMove
{
    protected abstract int GetNumSpaces();
    protected abstract Vector2Int GetDirection(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData);

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !target.Movable)
            return;
        Move(grid, user, target, GetDirection(grid, action, sub, user, target, targetData), GetNumSpaces());
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !target.Movable)
            return false;
        return CanMove(grid, user, target, GetDirection(grid, action, sub, user, target, targetData), GetNumSpaces());
    }
}
