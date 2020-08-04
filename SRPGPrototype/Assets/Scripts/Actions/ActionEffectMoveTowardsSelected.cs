using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveTowardsSelected : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        Move(grid, target, target.Pos.DirectionTo(targetData.selectedPos));
    }
}
