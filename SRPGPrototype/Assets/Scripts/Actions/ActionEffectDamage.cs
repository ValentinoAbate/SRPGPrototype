using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectDamage : ActionEffect
{
    private int baseDamage;

    public override void Initialize(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        baseDamage = BaseDamage(grid, action, user, targetPositions);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        int damage = baseDamage + TargetModifier(grid, action, user, target, targetData);
        target.Damage(damage);
        Debug.Log(target.DisplayName + " takes " + damage.ToString() + " damage and is now at " + target.HP + " hp");
    }

    public abstract int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions);

    public abstract int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData);
}
