using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMovePush : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        Move(grid, target, user.Pos.DirectionTo(targetData.targetPos));
    }
}
