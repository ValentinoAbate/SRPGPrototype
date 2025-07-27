using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveOverhead : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !target.Movable)
            return;
        SetPosition(grid, target, user.Pos - (target.Pos - user.Pos));
    }
}
