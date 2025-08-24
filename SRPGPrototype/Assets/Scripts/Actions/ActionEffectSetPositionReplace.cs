using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSetPositionReplace : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        var blocker = grid.Get(targetData.selectedPos);
        if (blocker == null)
            return;
        blocker.Kill(grid, user);
        grid.MoveAndSetWorldPos(target, targetData.selectedPos);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return grid.IsLegal(targetData.selectedPos) && !grid.IsEmpty(targetData.selectedPos) && target != null && target.Movable && target.Pos != targetData.selectedPos;
    }
}
