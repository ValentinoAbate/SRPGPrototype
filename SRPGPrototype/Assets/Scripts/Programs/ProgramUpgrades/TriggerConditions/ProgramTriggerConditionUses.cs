using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerConditionUses : ProgramTriggerCondition
{
    public override bool Completed => completed;
    private bool completed = false;

    [SerializeField] private int number = 5;
    [SerializeField] private Action.Trigger resetCount = Action.Trigger.Never;

    private int Progress
    {
        get
        {
            if (actions.Count <= 0)
                return 0;
            switch (resetCount)
            {
                case Action.Trigger.Never:
                    return actions.Max(ActionUtils.Uses);
                case Action.Trigger.TurnStart:
                    return actions.Max(ActionUtils.UsesTurn);
                case Action.Trigger.EncounterStart:
                    return actions.Max(ActionUtils.UsesEncounter);               
            }
            return 0;
        }
    }

    private readonly List<Action> actions = new List<Action>();

    public override string RevealedConditionText
    {
        get
        {
            return $"Use {number} {UsesText(resetCount)} ({(completed ? "Done" : Progress + "/" + number)})";
        }
    }

    public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        actions.Clear();
        // Log actions from the program
        actions.AddRange(program.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => (e as ProgramEffectAddAction).action));
        data.onAfterAction += Check;
    }

    private void Check(BattleGrid grid, Action action, Unit user, int cost)
    {
        if (completed || !actions.Contains(action))
            return;
        completed = Progress >= number;
    }

    public override string Save()
    {
        return BoolUtils.ToStringInt(completed);
    }

    public override void Load(string data)
    {
        completed = BoolUtils.FromStringInt(data);
    }
}
