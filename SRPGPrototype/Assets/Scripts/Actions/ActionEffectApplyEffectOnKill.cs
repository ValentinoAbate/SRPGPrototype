using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectApplyEffectOnKill : ActionEffect, IDamagingActionEffect
{
    public override bool UsesPower => effect.UsesPower;
    public bool DealsDamage => effect.CanDealDamage;

    [SerializeField] private ActionEffect effect;
    [SerializeField] private Unit.Team[] targetTeams;

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null || !target.Dead || !UnitFilters.IsOnTeam(target, targetTeams))
            return;
        effect.ApplyEffect(grid, action, sub, user, target, targetData);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return effect.IsValidTarget(grid, action, sub, user, target, targetData);
    }

    public int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        return effect is IDamagingActionEffect damageEffect ? damageEffect.ActionMacroDamage(grid, action, sub, user, indices) : 0;
    }

    public int CalculateDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData, bool simulation)
    {
        return effect is IDamagingActionEffect damageEffect ? damageEffect.CalculateDamage(grid, action, sub, user, target, targetData, simulation) : 0;
    }

    public override bool CanSave(bool isBattle)
    {
        return effect.CanSave(isBattle);
    }

    public override string Save(bool isBattle)
    {
        return effect.Save(isBattle);
    }

    public override void Load(string data, bool isBattle)
    {
        effect.Load(data, isBattle);
    }
}
