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

    public override bool UsesPower => true;
    public bool DealsDamage => true;
    private int baseDamage;

    public TargetStat DamageTarget => targetStat;
    [SerializeField] private TargetStat targetStat = TargetStat.HP;

    private readonly List<ModifierActionDamage> modifiers = new List<ModifierActionDamage>();
    private static IEnumerable<ModifierActionDamage> GetApplicableMods(Action action, SubAction sub)
    {
        if (action == null || action.Program == null)
            yield break;
        foreach(var mod in action.Program.ModifiedByType<ModifierActionDamage>())
        {
            if (mod.AppliesTo(sub))
                yield return mod;
        }
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
        int damage = CalculateDamage(grid, action, sub, user, target, targetData, false);

        // Apply damage
        if (targetStat == TargetStat.HP)
        {
#if DEBUG
            Debug.Log(target.DisplayName + " takes " + damage.ToString() + " damage and is now at " + (target.HP - damage) + " HP");
#endif
            target.Damage(grid, damage, user);
        }
        else if(targetStat == TargetStat.AP) // Target stat is AP
        {
#if DEBUG
            Debug.Log(target.DisplayName + " takes " + damage.ToString() + " AP damage and is now at " + (target.AP - damage) + " AP");
#endif
            target.AP -= damage;
        }
        else
        {
#if DEBUG
            Debug.Log(target.DisplayName + " heals " + damage.ToString() + " damage and is now at " + (target.HP + damage) + " HP");
#endif
            target.Heal(damage, user);
        }
    }

    protected sealed override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null;
    }

    public int CalculateDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData, bool simulation)
    {
        int damage = baseDamage + TargetModifier(grid, action, user, target, targetData);
        if (UsesPower)
        {
            damage += user.Power;
        }
        // Apply effect modifier target values
        foreach (var modifier in modifiers)
        {
            damage += modifier.TargetDamageMod(targetStat, grid, action, user, target, targetData);
        }
        // Apply unit incoming damage modifier base values
        if (target.IncomingDamageMods != null)
        {
            foreach (Unit.IncomingDamageMod modifier in target.IncomingDamageMods.GetInvocationList())
            {
                damage += modifier(grid, action, sub, target, user, damage, targetStat, simulation);
            }
        }
        // Make sure damage is non-negative
        return Mathf.Max(damage, 0);
    }

    public abstract int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions);

    public abstract int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData);

    public abstract int BasicDamage(Action action, Unit user);

    public int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        if(grid == null)
        {
            baseDamage = BasicDamage(action, user);
            // Apply modifier base damage
            foreach (var modifier in GetApplicableMods(action, sub))
            {
                baseDamage += modifier.BasicDamageMod(targetStat, action, user);
            }
            return baseDamage;
        }
        else
        {
            baseDamage = BaseDamage(grid, action, user, System.Array.Empty<Vector2Int>());
            // Apply modifier base damage
            foreach (var modifier in GetApplicableMods(action, sub))
            {
                baseDamage += modifier.BaseDamageMod(targetStat, grid, action, user, System.Array.Empty<Vector2Int>());
            }
            return baseDamage;
        }
    }
}
