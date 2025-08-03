using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectHeal : ActionEffect
{
    public override bool UsesPower => true;
    private int baseDamage;

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        baseDamage = BaseDamage(grid, action, user, targetPositions);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        int damage = Mathf.Max(baseDamage + TargetModifier(grid, action, user, target, targetData) + user.Power.Value, 0);
        Debug.Log(target.DisplayName + " heals " + damage.ToString() + " damage and is now at " + (target.HP + damage) + " HP");
        target.Heal(damage, user);
    }

    public override bool IsValidTarget(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null;
    }

    public abstract int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions);

    public abstract int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData);
}
