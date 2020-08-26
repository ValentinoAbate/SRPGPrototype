using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectFeedback : ProgramEffectAddOnAfterSubActionAbility
{
    public Stats.StatName stat;
    public ProgramNumber number;
    public override void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        if (subAction.DealsDamage && targets.Contains(user))
            user.ModifyStat(grid, stat, number.Value(action.Program), user);
    }
}
