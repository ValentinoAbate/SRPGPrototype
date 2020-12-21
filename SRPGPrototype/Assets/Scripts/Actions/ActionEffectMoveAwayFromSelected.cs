using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveAwayFromSelected : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        Move(grid, target, targetData.selectedPos.DirectionTo(target.Pos));
    }
}
