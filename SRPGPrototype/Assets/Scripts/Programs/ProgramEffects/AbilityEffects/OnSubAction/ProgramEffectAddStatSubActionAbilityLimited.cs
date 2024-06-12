using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatSubActionAbilityLimited : ProgramEffectAddStatSubActionAbility
{
    [SerializeField] private UseLimiter limiter;
    public override void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        if (!limiter.TryUse())
        {
            return;
        }
        base.OnSubAction(grid, action, subAction, user, targets, targetPositions);
    }
}
