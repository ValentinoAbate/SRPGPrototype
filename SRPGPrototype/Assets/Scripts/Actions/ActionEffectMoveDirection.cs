using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveDirection : ActionEffectMove
{
    public Vector2Int direction;

    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        Move(grid, target, direction);
    }
}
