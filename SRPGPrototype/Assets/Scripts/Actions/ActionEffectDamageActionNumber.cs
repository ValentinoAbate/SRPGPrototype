using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageActionNumber : ActionEffectDamage
{
    public ActionNumber damage;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        return damage.ActionValue(grid, action, user, targetPositions);
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return damage.TargetValue(grid, action, user, target, targetData);
    }
}
