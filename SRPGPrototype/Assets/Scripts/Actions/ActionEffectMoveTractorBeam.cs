using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveTractorBeam : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        var direction = target.Pos.DirectionTo(user.Pos);
        var goal = user.Pos - direction;
        while (target.Pos != goal)
        {
            if (!Move(grid, target, direction))
                break;
        }
    }
}
