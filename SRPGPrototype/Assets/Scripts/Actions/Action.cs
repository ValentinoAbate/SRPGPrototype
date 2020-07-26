using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Action : MonoBehaviour, IEnumerable<SubAction>
{
    public enum Type
    { 
        Move,
        Standard,
        Hybrid,
    }

    public enum Trigger
    { 
        Never,
        TurnStart,
        EncounterStart,
    }

    public bool UsesPower => subActions.Any((s) => s.UsesPower);

    public bool Usable => true;

    public Program Program { get; set; }

    public Type ActionType => type;
    [SerializeField] Type type = Type.Standard;

    public int APCost 
    {
        get
        {
            int usage = TimesUsed;
            if (SlowdownReset == Trigger.TurnStart)
                usage = TimesUsedThisTurn;
            else if (SlowdownReset == Trigger.EncounterStart)
                usage = TimesUsedThisBattle;
            return baseAp + Slowdown * (usage / SlowdownInterval);
        }
    }
    [SerializeField] private int baseAp = 1;

    public int Slowdown => slowdown;
    [SerializeField] private int slowdown = 1;

    public int SlowdownInterval => slowdownInterval;
    [SerializeField] private int slowdownInterval = 1;

    public Trigger SlowdownReset => slowdownReset;
    [SerializeField] private Trigger slowdownReset = Trigger.TurnStart;

    public int TimesUsed { get; set; } = 0;

    public int TimesUsedThisBattle { get; set; } = 0;

    public int TimesUsedThisTurn { get; set; } = 0;

    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    [HideInInspector]
    public List<SubAction> subActions;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        var singleAction = GetComponent<SubAction>();
        subActions = singleAction != null ? new List<SubAction> { singleAction } :
            new List<SubAction>(GetComponentsInChildren<SubAction>());
    }

    public void Use(Unit user)
    {
        user.AP -= APCost;
        ++TimesUsed;
        ++TimesUsedThisBattle;
        ++TimesUsedThisTurn;
        if (UsesPower)
        {
            user.Power.Use();
        }
        if (Program == null)
            return;
        if(Program.attributes.HasFlag(Program.Attributes.Transient))
        {
            Program.GetComponent<ProgramAttributeTransient>().Uses++;
        }
    }

    public void CopyTemporaryValues(Action other)
    {
        TimesUsedThisTurn = other.TimesUsedThisTurn;
        TimesUsedThisBattle = other.TimesUsedThisBattle;
        TimesUsed = other.TimesUsed;
    }

    public IEnumerator<SubAction> GetEnumerator()
    {
        return subActions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return subActions.GetEnumerator();
    }
}
