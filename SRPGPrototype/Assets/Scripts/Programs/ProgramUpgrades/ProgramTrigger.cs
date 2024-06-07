using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramTrigger : MonoBehaviour
{
    public ProgramTriggerCondition Condition => GetComponent<ProgramTriggerCondition>();

    public bool Hidden { get => hidden; }
    [SerializeField] private bool hidden = false;
    public string TriggerName { get => Hidden ? ProgramDescriptionUI.Hide(displayName) : displayName; }
    [SerializeField] protected string displayName = string.Empty;

    public abstract void Initialize(Program program);
}
