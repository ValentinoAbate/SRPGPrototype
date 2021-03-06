﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectMovePull : ActionEffectMove
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        Move(grid, target, targetData.targetPos.DirectionTo(user.Pos));
    }
}
