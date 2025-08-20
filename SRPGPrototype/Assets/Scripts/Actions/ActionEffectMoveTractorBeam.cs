using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveTractorBeam : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        var goal = user.Pos - target.Pos.DirectionTo(user.Pos);
        SetPosition(grid, user, target, goal);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null && target.Movable && (target.Pos + target.Pos.DirectionTo(user.Pos) != user.Pos);
    }
}
