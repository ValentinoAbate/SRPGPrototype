using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProgramTriggerConditionResetTrigger : ProgramTriggerCondition
{
    sealed public override bool Completed => completed;
    private bool completed = false;

    [SerializeField] private int number = 5;
    [SerializeField] private Action.Trigger resetTrigger = Action.Trigger.Never;

    private readonly List<Action> actions = new List<Action>();
    protected int Progress { get; private set; } = 0;
    private int turnUses = 0;
    private int encounterUses = 0;

    protected abstract string ProgressConditionText { get; }

    sealed public override string RevealedConditionText
    {
        get
        {
            CheckResetUses();
            if(number <= 1)
            {
                return completed ? $"{ProgressConditionText} (Done)" : ProgressConditionText;
            }
            return $"{ProgressConditionText} {number} {UsesText(resetTrigger)} ({(completed ? "Done" : Progress + "/" + number)})";
        }
    }

    sealed public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        actions.Clear();
        // Log actions from the program
        foreach(var effect in program.Effects)
        {
            if(effect is ProgramEffectAddAction addAction)
            {
                actions.Add(addAction.action);
            }
        }
        // Add the check
        data.onAfterSubAction += Check;
        data.onAfterAction += UpdateUses;
    }
    protected abstract int ProgressChange(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions);
    private void Check(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions, SubAction.Options options, SubAction.Type overrideSubType = SubAction.Type.None)
    {
        if (completed || options.HasFlag(SubAction.Options.SkipUpgradeCheck) || !actions.Contains(action))
            return;
        CheckResetUses();
        Progress += ProgressChange(grid, action, subAction, user, targets, targetPositions);
        completed = Progress >= number;
    }
    private void CheckResetUses()
    {
        if (resetTrigger == Action.Trigger.TurnStart && actions.Sum(ActionUtils.UsesTurn) != turnUses)
        {
            turnUses = 0;
            Progress = 0;
        }
        else if (resetTrigger == Action.Trigger.EncounterStart && actions.Sum(ActionUtils.UsesEncounter) != encounterUses)
        {
            encounterUses = 0;
            Progress = 0;
        }
    }

    private void UpdateUses(BattleGrid grid, Action action, Unit user, int cost)
    {
        if (completed || !actions.Contains(action))
            return;
        ++turnUses;
        ++encounterUses;
    }

    public override string Save()
    {
        return $"{(completed ? 1 : 0)},{Progress},{turnUses},{encounterUses}";
    }

    public override void Load(string data)
    {
        var args = data.Split(SaveManager.separator);
        try
        {
            if(int.TryParse(args[0], out int completedInt))
            {
                completed = completedInt != 0;
            }
            if(int.TryParse(args[1], out int progress))
            {
                Progress = progress;
            }
            int.TryParse(args[2], out turnUses);
            int.TryParse(args[3], out encounterUses);
        }
        catch(System.Exception e)
        {
            Debug.LogError($"ProgramTriggerConditionResetTrigger Load Error: {e}");
        }
    }
}
