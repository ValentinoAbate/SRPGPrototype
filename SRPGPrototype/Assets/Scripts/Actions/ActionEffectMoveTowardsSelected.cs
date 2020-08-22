using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveTowardsSelected : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        Move(grid, target, target.Pos.DirectionTo(targetData.selectedPos));
    }
}
