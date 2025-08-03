using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectHitRandomUnit : ActionEffect, IDamagingActionEffect, IGambleActionEffect
{
    public override bool UsesPower => effectToApply.UsesPower;
    public bool DealsDamage => effectToApply.CanDealDamage;
    public bool? GambleSuccess { get; private set; } = null;

    [SerializeField] private ActionEffect effectToApply;
    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };
    [SerializeField] private bool gambleFailOnHitAlly;

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        effectToApply.Initialize(grid, action, sub, user, targetPositions);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        var targets = grid.FindAll((u) => teams.Contains(u.UnitTeam));
        if (targets.Count <= 0)
        {
            GambleSuccess = null;
            return;
        }
        var newTarget = RandomUtils.RandomU.instance.Choice(targets);
        var newTargetData = new PositionData() { targetPos = newTarget.Pos, selectedPos = targetData.selectedPos };
        if(gambleFailOnHitAlly && newTarget.UnitTeam == user.UnitTeam)
        {
            GambleSuccess = false;
        }
        else
        {
            GambleSuccess = null;
        }
        effectToApply.ApplyEffect(grid, action, sub, user, newTarget, newTargetData);
    }

    public int ActionMacroDamage(Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        if (effectToApply is IDamagingActionEffect damageEffect)
        {
            return damageEffect.ActionMacroDamage(action, sub, user, indices);
        }
        return 0;
    }
}
