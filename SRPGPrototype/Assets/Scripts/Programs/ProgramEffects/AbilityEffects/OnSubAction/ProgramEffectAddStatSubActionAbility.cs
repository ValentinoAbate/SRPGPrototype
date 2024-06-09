using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramEffectAddStatSubActionAbility : ProgramEffectAddSubActionAbility
{
    public Stats.StatName stat;
    public ActionNumber number;
    protected override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;

    [SerializeField] private SubAction.Type[] subTypes = new SubAction.Type[0];
    public override void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        if (subTypes.Length > 0 && !subTypes.Contains(subAction.Subtype))
        {
            return;
        }
        int value = number.ActionValue(grid, action, user, targets.Count);
        user.ModifyStat(grid, stat, value, user);
    }
}
