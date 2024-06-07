using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageBasic : ActionEffectDamage
{
    public int damage = 2;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return damage;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }

    public override int BasicDamage(Action action, Unit user)
    {
        return damage;
    }
}
