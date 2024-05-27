using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMovePush : ActionEffectMove
{
    [SerializeField] private int numSpaces = 1;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        for(int i = 0; i < numSpaces; ++i)
        {
            Move(grid, target, user.Pos.DirectionTo(targetData.targetPos));
        }
    }
}
