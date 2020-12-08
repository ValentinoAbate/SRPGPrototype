using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSetPosition : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        grid.MoveAndSetWorldPos(target, targetData.selectedPos);
    }
}
