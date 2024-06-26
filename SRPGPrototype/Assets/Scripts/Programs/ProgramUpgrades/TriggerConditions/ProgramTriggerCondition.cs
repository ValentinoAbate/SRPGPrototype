using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramTriggerCondition : MonoBehaviour
{
    public const string hiddenText = "??????????????";
    public bool Hidden { get => hidden; }
    [SerializeField] private bool hidden = false;
    public string ConditionText { get => Hidden && !Completed ? hiddenText : RevealedConditionText; }
    public abstract string RevealedConditionText { get; }
    public abstract bool Completed { get; }

    /// <summary>
    /// Should be called when the shell is compiled and then whenever an upgrade happens.
    /// </summary>
    public abstract void LinkEffect(Program program, ref Shell.CompileData data);

    protected string UsesText(Action.Trigger resetTrigger)
    {
        switch (resetTrigger)
        {

            case Action.Trigger.TurnStart:
                return "times this turn";
            case Action.Trigger.EncounterStart:
                return "times this encounter";
            case Action.Trigger.Never:
            default:
                return "times";
        }
    }
}
