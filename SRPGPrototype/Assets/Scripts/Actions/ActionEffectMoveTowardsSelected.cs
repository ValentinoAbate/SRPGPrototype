using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveTowardsSelected : ActionEffectMoveDirectionBasic
{
    protected override Vector2Int GetDirection(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target.Pos.DirectionTo(targetData.selectedPos);
    }
}
