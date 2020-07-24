using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageBasic : ActionEffectDamage
{
    public int damage = 2;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        return damage;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
