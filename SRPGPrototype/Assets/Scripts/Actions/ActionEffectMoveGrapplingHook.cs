using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveGrapplingHook : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        var direction = target.Pos.DirectionTo(targetData.selectedPos);
        var goal = targetData.selectedPos - direction;
        while(user.Pos != goal)
        {
            if(!Move(grid, user, direction))
                break;
        }
    }
}
