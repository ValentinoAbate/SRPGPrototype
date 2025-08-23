using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramEffectAddStatSubActionAbility : ProgramEffectAddSubActionAbility
{
    public Stats.StatName stat;
    public ActionNumber number;
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;

    public override void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        int value = number.ActionValue(grid, action, user, targets.Count);
        user.ModifyStat(grid, stat, value, user);
    }
}
