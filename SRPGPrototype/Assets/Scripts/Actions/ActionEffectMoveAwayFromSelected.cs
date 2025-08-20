using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMoveAwayFromSelected : ActionEffectMoveDirectionBasic
{
    protected override Vector2Int GetDirection(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return targetData.selectedPos.DirectionTo(target.Pos);
    }
}
