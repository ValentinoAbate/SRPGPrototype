using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnGamble : ProgramEffectAddSubActionAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private int quantity;
    [SerializeField] private bool success;
    protected override string AbilityName => $"Add {quantity} {stat} after Gamble {(success ? "Success" : "Fail")}.";

    public override void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        int occurrences = 0;
        foreach(var effect in subAction.Effects)
        {
            if(effect is IGambleActionEffect gambleActionEffect && gambleActionEffect.GambleSuccess == success)
            {
                occurrences++;
            }
        }
        if(occurrences > 0)
        {
            user.ModifyStat(grid, stat, quantity * occurrences, user);
        }
    }
}
