using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveDirection : ActionEffectMove
{
    public Vector2Int direction;

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        Move(grid, target, direction);
    }
}
