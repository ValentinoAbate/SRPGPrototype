using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ActionEffectDamage : ActionEffect
{
    public enum TargetStat
    { 
        HP,
        AP,
        HealHP,
    }

    public override bool UsesPower => true;
    private int baseDamage;

    public TargetStat DamageTarget => targetStat;
    [SerializeField] private TargetStat targetStat = TargetStat.HP;

    private readonly List<ModifierActionDamage> modifiers = new List<ModifierActionDamage>();

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        modifiers.Clear();
        if(action.Program != null)
        {
            modifiers.AddRange(action.Program.ModifiedBy.Where((mod) => mod is ModifierActionDamage)
                                                       .Select((mod) => mod as ModifierActionDamage)
                                                       .Where((mod) => mod.AppliesTo(sub)));
        }
        baseDamage = BaseDamage(grid, action, user, targetPositions);
        // Apply modifier base damage
        foreach(var modifier in modifiers)
        {
            baseDamage += modifier.DamageModifiers.Sum((mod) => mod.BaseDamage(grid, action, user, targetPositions));
        }
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;

        // Calculate final damage

        int damage = baseDamage + TargetModifier(grid, action, user, target, targetData) + user.Power.Value;
        // Apply modifier target values
        foreach (var modifier in modifiers)
        {
            damage += modifier.DamageModifiers.Sum((mod) => mod.TargetModifier(grid, action, user, target, targetData));
        }
        // Make sure damage is non-negative
        damage = Mathf.Max(damage, 0);

        // Apply damage

        if (targetStat == TargetStat.HP)
        {
            Debug.Log(target.DisplayName + " takes " + damage.ToString() + " damage and is now at " + (target.HP - damage) + " HP");
            target.Damage(grid, damage, user);
        }
        else if(targetStat == TargetStat.AP) // Target stat is AP
        {
            Debug.Log(target.DisplayName + " takes " + damage.ToString() + " AP damage and is now at " + (target.AP - damage) + " AP");
            target.AP -= damage;
        }
        else
        {
            Debug.Log(target.DisplayName + " heals " + damage.ToString() + " damage and is now at " + (target.HP + damage) + " HP");
            target.Heal(damage, user);
        }
    }

    public abstract int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions);

    public abstract int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData);
}
