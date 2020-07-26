using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectDamage : ActionEffect
{
    public enum TargetStat
    { 
        HP,
        AP,
    }

    public override bool UsesPower => true;
    private int baseDamage;

    public TargetStat DamageTarget => targetStat;
    [SerializeField] private TargetStat targetStat = TargetStat.HP;

    public override void Initialize(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        baseDamage = BaseDamage(grid, action, user, targetPositions);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        int damage = Mathf.Max(baseDamage + TargetModifier(grid, action, user, target, targetData) + user.Power.Value, 0);
        if(targetStat == TargetStat.HP)
        {
            Debug.Log(target.DisplayName + " takes " + damage.ToString() + " damage and is now at " + (target.HP - damage) + " HP");
            target.Damage(damage);
        }
        else // Target stat is AP
        {
            Debug.Log(target.DisplayName + " takes " + damage.ToString() + " AP damage and is now at " + (target.AP - damage) + " AP");
            target.AP -= damage;
        }
    }

    public abstract int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions);

    public abstract int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData);
}
