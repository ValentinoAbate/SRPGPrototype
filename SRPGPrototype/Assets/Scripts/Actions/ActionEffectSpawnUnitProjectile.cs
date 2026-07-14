using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSpawnUnitProjectile : ActionEffectSpawnUnit
{
    public Unit unitPrefab;

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if(grid.TryGet(targetData.targetPos, out var unit) && !unit.Dead)
        {
            var newTarget = targetData.targetPos + targetData.targetPos.DirectionTo(user.Pos);
            base.ApplyEffect(grid, action, sub, user, target, new PositionData(newTarget, targetData.selectedPos, targetData.originalUserPos));
            return;
        }
        base.ApplyEffect(grid, action, sub, user, target, targetData);
    }

    protected override GameObject GetUnitPrefab()
    {
        return unitPrefab.gameObject;
    }
}
