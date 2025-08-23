using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectFeedback : ProgramEffectAddSubActionAbility
{
    public Stats.StatName stat;
    public ProgramNumber number;
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;
    public override void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        if (subAction.DealsDamage && targets.Contains(user))
            user.ModifyStat(grid, stat, number.Value(action.Program), user);
    }
}
