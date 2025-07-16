using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramTriggerConditionGambles : ProgramTriggerConditionResetTrigger
{
    [SerializeField] private bool success;
    protected override string ProgressConditionText => $"{(success ? "Succeed at" : "Fail")} gambles";

    protected override int ProgressChange(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        int progressGained = 0;
        foreach(var effect in subAction.Effects)
        {
            if(effect is ActionEffectGamble gambleEffect && gambleEffect.LastUsageWasSuccessful == success)
            {
                ++progressGained;
            }
        }
        return progressGained;
    }
}
