using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSetPosition : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, Combatant user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        grid.MoveAndSetWorldPos(target, targetData.selectedPos);
    }
}
