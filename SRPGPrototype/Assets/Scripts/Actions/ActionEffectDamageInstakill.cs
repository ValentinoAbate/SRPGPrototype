using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageInstakill : ActionEffectDamage
{
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return 0;
    }

    public override int BasicDamage(Action action, Unit user)
    {
        return 0;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return target.HP;
    }
}
