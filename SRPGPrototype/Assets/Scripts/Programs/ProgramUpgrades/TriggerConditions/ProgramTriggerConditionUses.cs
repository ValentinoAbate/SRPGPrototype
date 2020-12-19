using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerConditionUses: ProgramTriggerCondition
{
    public override bool Completed => completed;
    private bool completed = false;

    [SerializeField] private int number = 5;
    [SerializeField] private Action.Trigger resetCount;

    private int Progress
    {
        get
        {
            if (actions.Count <= 0)
                return 0;
            switch (resetCount)
            {
                case Action.Trigger.Never:
                    return actions.Max((a) => a.TimesUsed);
                case Action.Trigger.TurnStart:
                    return actions.Max((a) => a.TimesUsedThisTurn);
                case Action.Trigger.EncounterStart:
                    return actions.Max((a) => a.TimesUsedThisBattle);               
            }
            return 0;
        }
    }

    private readonly List<Action> actions = new List<Action>();

    public override string RevealedConditionText
    {
        get
        {
            string progressText = "(" + (completed ? "Done" : Progress + "/" + number) + ")";
            string conditionText = "Use " + number;
            return conditionText + UsesText(resetCount) + progressText;
        }
    }

    public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        actions.Clear();
        // Log actions from the program
        actions.AddRange(program.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => (e as ProgramEffectAddAction).action));
        data.onAfterAction += Check;
    }

    private void Check(Action action)
    {
        if (completed || !actions.Contains(action))
            return;
        completed = Progress >= number;
    }
}
