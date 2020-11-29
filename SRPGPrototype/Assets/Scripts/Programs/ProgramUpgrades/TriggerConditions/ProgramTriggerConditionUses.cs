using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerConditionUses: ProgramTriggerCondition
{
    public override bool Completed => Progress >= number;

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
            switch (resetCount)
            {

                case Action.Trigger.TurnStart:
                    return "Use " + number + " times this turn (" + Progress + "/" + number + ")";
                case Action.Trigger.EncounterStart:
                    return "Use " + number + " times this encounter (" + Progress + "/" + number + ")";
                case Action.Trigger.Never:
                default:
                    return "Use " + number + " times (" + Progress + "/" + number + ")";
            }

        }

    }

    public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        actions.Clear();
        // Log actions from the program
        actions.AddRange(program.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => (e as ProgramEffectAddAction).action));
    }
}
