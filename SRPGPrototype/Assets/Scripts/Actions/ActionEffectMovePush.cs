using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMovePush : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        Move(grid, target, user.Pos.DirectionTo(targetData.targetPos));
    }
}
