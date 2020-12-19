using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProgramTriggerConditionResetTrigger : ProgramTriggerCondition
{
    public override bool Completed => completed;
    private bool completed = false;

    [SerializeField] private int number = 5;
    [SerializeField] private Action.Trigger resetTrigger = Action.Trigger.Never;

    private readonly List<Action> actions = new List<Action>();
    private int progress = 0;
    private int turnUses = 0;
    private int encounterUses = 0;

    protected abstract string ProgressConditionText { get; }

    public override string RevealedConditionText
    {
        get
        {
            string progressText = "(" + (completed ? "Done" : progress + "/" + number) + ")";
            return ProgressConditionText + number + UsesText(resetTrigger) + progressText;
        }
    }

    public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        actions.Clear();
        // Log actions from the program
        actions.AddRange(program.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => (e as ProgramEffectAddAction).action));
        // Add the check
        data.onAfterSubAction += Check;
        data.onAfterAction += UpdateUses;
    }
    protected abstract int ProgressChange(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions);
    private void Check(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        if (!actions.Contains(action))
            return;
        if(resetTrigger == Action.Trigger.TurnStart && actions.Sum((a) => a.TimesUsedThisTurn) != turnUses)
        {
            turnUses = 0;
            progress = ProgressChange(grid, action, subAction, user, targets, targetPositions);
        }
        else if(resetTrigger == Action.Trigger.EncounterStart && actions.Sum((a) => a.TimesUsedThisBattle) != encounterUses)
        {
            encounterUses = 0;
            progress = ProgressChange(grid, action, subAction, user, targets, targetPositions); ;
        }
        else
        {
            progress += ProgressChange(grid, action, subAction, user, targets, targetPositions);
        }
        completed = progress >= number;
    }
    private void UpdateUses(Action a)
    {
        ++turnUses;
        ++encounterUses;
    }
}
