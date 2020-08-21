using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterSubAction : ProgramEffectAddOnAfterSubActionAbility
{
    public Stats.StatName stat;
    public ActionNumber number;
    public override void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Vector2Int> targetPositions)
    {
        int value = number.ActionValue(grid, action, user, targetPositions);
        user.ModifyStat(grid, stat, value, user);
    }
}
