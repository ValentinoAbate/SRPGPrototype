using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ActionEffectDamage : ActionEffect, IDamagingActionEffect
{
    public enum TargetStat
    { 
        HP,
        AP,
        HealHP,
    }

    sealed public override bool UsesPower => true;
    public bool DealsDamage => true;
    private int baseDamage;

    public TargetStat DamageTarget => targetStat;
    [SerializeField] private TargetStat targetStat = TargetStat.HP;

    private readonly List<ModifierActionDamage> modifiers = new List<ModifierActionDamage>();
    private static IEnumerable<ModifierActionDamage> GetApplicableMods(Action action, SubAction sub)
    {
        if (action.Program == null)
            return System.Array.Empty<ModifierActionDamage>();
        return action.Program.ModifiedByType<ModifierActionDamage>().Where((mod) => mod.AppliesTo(sub));
    }

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        modifiers.Clear();
        if(action.Program != null)
        {
            modifiers.AddRange(GetApplicableMods(action, sub));
        }
        baseDamage = BaseDamage(grid, action, user, targetPositions);
        // Apply modifier base damage
        foreach(var modifier in modifiers)
        {
            baseDamage += modifier.BaseDamageMod(targetStat, grid, action, user, targetPositions);
        }
    }

    sealed public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;

        // Calculate final damage

        int damage = baseDamage + TargetModifier(grid, action, user, target, targetData) + user.Power.Value;
        // Apply effect modifier target values
        foreach (var modifier in modifiers)
        {
            damage += modifier.TargetDamageMod(targetStat, grid, action, user, target, targetData);
        }
        // Apply unit incoming damage modifier base values
        foreach (var modifier in target.IncomingDamageModifiers)
        {
            damage += modifier.IncomingDamageMod(targetStat, grid, action, sub, target, System.Array.Empty<Vector2Int>());
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

    public abstract int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions);

    public abstract int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData);

    public abstract int BasicDamage(Action action, Unit user);

    public int BaseDamage(Action action, SubAction sub, Unit user, params int[] indices)
    {
        baseDamage = BasicDamage(action, user);
        // Apply modifier base damage
        foreach (var modifier in GetApplicableMods(action, sub))
        {
            baseDamage += modifier.BasicDamageMod(targetStat, action, user);
        }
        return baseDamage;
    }
}
