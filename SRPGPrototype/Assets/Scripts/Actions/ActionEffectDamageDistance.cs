using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageDistance : ActionEffectDamage
{
    [SerializeField] private DynamicNumber modifiers;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return 0;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return modifiers.Value(Math.Max(Math.Abs(user.Pos.x - target.Pos.x), Math.Abs(user.Pos.y - target.Pos.y)));
    }

    public override int BasicDamage(Action action, Unit user)
    {
        return 0;
    }
}
