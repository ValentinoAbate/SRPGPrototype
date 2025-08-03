using System.Collections.Generic;
using UnityEngine;

public class ModifierActionDamage : ModifierAction
{
    public ActionEffectDamage[] DamageModifiers { get; private set; }
    [SerializeField] private GameObject actionEffectContainer;



    private void Awake()
    {
        if(actionEffectContainer != null)
        {
            DamageModifiers = actionEffectContainer.GetComponents<ActionEffectDamage>();
        }
        else
        {
            DamageModifiers = GetComponents<ActionEffectDamage>();
        }

    }

    public override bool AppliesTo(SubAction sub)
    {
        return sub.DealsDamage;
    }

    public int BaseDamageMod(ActionEffectDamage.TargetStat targetStat, BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        int sum = 0;
        foreach(var mod in DamageModifiers)
        {
            if (mod.DamageTarget != targetStat)
                continue;
            sum += mod.BaseDamage(grid, action, user, targetPositions);
        }
        return sum;
    }

    public int TargetDamageMod(ActionEffectDamage.TargetStat targetStat, BattleGrid grid, Action action, Unit user, Unit target, ActionEffect.PositionData targetData)
    {
        int sum = 0;
        foreach (var mod in DamageModifiers)
        {
            if (mod.DamageTarget != targetStat)
                continue;
            sum += mod.TargetModifier(grid, action, user, target, targetData);
        }
        return sum;
    }

    public int BasicDamageMod(ActionEffectDamage.TargetStat targetStat, Action action, Unit user)
    {
        int sum = 0;
        foreach (var mod in DamageModifiers)
        {
            if (mod.DamageTarget != targetStat)
                continue;
            sum += mod.BasicDamage(action, user);
        }
        return sum;
    }
}
