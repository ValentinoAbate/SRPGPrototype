using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramTriggerCondition : MonoBehaviour
{
    public const string hiddenText = "???????????";
    public bool Hidden { get => hidden; }
    [SerializeField] private bool hidden = false;
    public string ConditionText { get => Hidden ? hiddenText : RevealedConditionText; }
    public abstract string RevealedConditionText { get; }
    public abstract bool Completed { get; }

    /// <summary>
    /// Should be called when the shell is compiled and then whenever an upgrade happens.
    /// </summary>
    public abstract void LinkEffect(Program program, ref Shell.CompileData data);
}
