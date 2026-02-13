using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectHitRandomUnit : ActionEffect, IDamagingActionEffect
{
    public override bool UsesPower => effectToApply.UsesPower;
    public bool DealsDamage => effectToApply.CanDealDamage;

    [SerializeField] private ActionEffect effectToApply;
    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };
    [SerializeField] private Unit.Team[] desiredTeams = new Unit.Team[] { Unit.Team.Enemy };

    private bool OnTargetTeam(Unit u) => teams.Contains(u.UnitTeam);

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        effectToApply.Initialize(grid, action, sub, user, targetPositions);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        var targets = grid.FindAll(OnTargetTeam);
        if (targets.Count <= 0)
        {
            return;
        }
        var newTarget = RandomUtils.RandomU.instance.Choice(targets);
        var newTargetData = new PositionData() { targetPos = newTarget.Pos, selectedPos = targetData.selectedPos };
        effectToApply.ApplyEffect(grid, action, sub, user, newTarget, newTargetData);
        if (desiredTeams.Length > 0)
        {
            user.OnGambleFn?.Invoke(grid, action, user, UnitFilters.IsOnTeam(newTarget, desiredTeams));
        }
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return grid.FindAll(OnTargetTeam).Count > 0;
    }

    public int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        if (effectToApply is IDamagingActionEffect damageEffect)
        {
            return damageEffect.ActionMacroDamage(grid, action, sub, user, indices);
        }
        return 0;
    }

    public int CalculateDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData, bool simulation)
    {
        if (effectToApply is IDamagingActionEffect damageEffect)
        {
            return damageEffect.CalculateDamage(grid, action, sub, user, target, targetData, simulation);
        }
        return 0;
    }

    public override bool CanSave(bool isBattle) => effectToApply.CanSave(isBattle);

    public override string Save(bool isBattle) => effectToApply.Save(isBattle);

    public override void Load(string data, bool isBattle) => effectToApply.Load(data, isBattle);
}
