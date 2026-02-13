using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PorgramTriggerConditionOnGamble : ProgramTriggerCondition
{
    private readonly List<Action> actions = new List<Action>();

    public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        if (Completed)
            return;
        data.onGamble += Check;
        actions.Clear();
        // Log actions from the program
        foreach (var effect in program.Effects)
        {
            if (effect is ProgramEffectAddAction addActionEffect)
            {
                actions.Add(addActionEffect.action);
            }
        }
    }

    private void Check(BattleGrid grid, Action action, Unit unit, bool success)
    {
        if(!Completed && action != null && actions.Contains(action))
        {
            OnGamble(grid, action, unit, success);
        }
    }

    protected abstract void OnGamble(BattleGrid grid, Action action, Unit unit, bool gambleSuccess);
}
