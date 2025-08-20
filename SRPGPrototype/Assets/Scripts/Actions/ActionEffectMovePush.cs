using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMovePush : ActionEffectMoveDirectionBasic
{
    protected override Vector2Int GetDirection(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return user.Pos.DirectionTo(targetData.targetPos);
    }
}
