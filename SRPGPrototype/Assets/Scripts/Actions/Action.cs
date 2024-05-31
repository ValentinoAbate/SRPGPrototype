using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RandomUtils;
using System;

public class Action : MonoBehaviour, IEnumerable<SubAction>, IComparable<Action>
{
    public enum Type
    { 
        Move,
        Weapon,
        Hybrid,
        Skill,
    }

    public enum Trigger
    { 
        Never,
        TurnStart,
        EncounterStart,
    }

    public bool UsesPower => subActions.Any((s) => s.UsesPower);

    public Program Program { get; set; }

    public Type ActionType => type;
    [SerializeField] Type type = Type.Weapon;

    public int APCost 
    {
        get
        {
            int usage = TimesUsed - FreeUses;
            if (SlowdownReset == Trigger.TurnStart)
                usage = TimesUsedThisTurn - FreeUsesThisTurn;
            else if (SlowdownReset == Trigger.EncounterStart)
                usage = TimesUsedThisBattle - FreeUsesThisBattle;
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

    public int TimesUsed { get; private set; } = 0;
    public int FreeUses { get; private set; } = 0;

    public int TimesUsedThisBattle { get; private set; } = 0;
    public int FreeUsesThisBattle { get; private set; } = 0;

    public int TimesUsedThisTurn { get; private set; } = 0;
    public int FreeUsesThisTurn { get; private set; } = 0;

    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public string Description => description;
    [SerializeField] [TextArea(1, 3)] private string description = string.Empty;

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
            new List<SubAction>(GetComponentsInChildren<SubAction>(true));
    }

    public Action Validate(Transform parent)
    {
        if (transform.IsChildOf(parent))
            return this;
        return Instantiate(gameObject, parent).GetComponent<Action>();
    }

    public void ResetUses(Trigger trigger)
    {
        switch (trigger)
        {
            case Trigger.Never:
                TimesUsed = 0;
                FreeUses = 0;
                goto case Trigger.EncounterStart;
            case Trigger.EncounterStart:
                TimesUsedThisBattle = 0;
                FreeUsesThisBattle = 0;
                goto case Trigger.TurnStart;
            case Trigger.TurnStart:
                TimesUsedThisTurn = 0;
                FreeUsesThisTurn = 0;
                break;
        }

    }

    public int APCostAfterXUses(int uses)
    {
        int baseUsage = TimesUsed;
        if (SlowdownReset == Trigger.TurnStart)
            baseUsage = TimesUsedThisTurn;
        else if (SlowdownReset == Trigger.EncounterStart)
            baseUsage = TimesUsedThisBattle;
        return baseAp + Slowdown * ((baseUsage + uses) / SlowdownInterval);
    }

    private bool zeroPower = false;
    private bool zeroSpeed = false;

    public void UseAll(BattleGrid grid, Unit user, Vector2Int targetPos, bool applyAPCost = true)
    {
        StartAction(user);
        foreach (var sub in subActions)
        {
            sub.Use(grid, this, user, targetPos);
        }
        FinishAction(user, applyAPCost);
    }

    public void StartAction(Unit user)
    {
        zeroPower = user.Power.IsZero;
        zeroSpeed = user.Speed.IsZero;
    }

    public void FinishAction(Unit user, bool applyAPCost = true)
    {
        if(applyAPCost)
        {
            int cost = APCost;
            if(!zeroSpeed)
            {
                cost -= user.Speed.Value;
                user.Speed.Use();
            }
            user.AP -= Mathf.Max(cost, 0);
        }
        ++TimesUsed;
        ++TimesUsedThisBattle;
        ++TimesUsedThisTurn;
        if (UsesPower && !zeroPower)
        {
            user.Power.Use();
        }
        if(Program != null)
        {
            var noSlowdownMods = Program.ModifiedByType<ModifierActionNoSlowdownChance>();
            if (noSlowdownMods.Count() > 0 && RandomU.instance.RandomDouble() < noSlowdownMods.Sum((m) => m.Chance))
            {
                ++FreeUses;
                ++FreeUsesThisBattle;
                ++FreeUsesThisTurn;
            }
            if(Program.attributes.HasFlag(Program.Attributes.Transient))
            {
                var freeTransientMods = Program.ModifiedByType<ModifierActionFreeTransientChance>();
                // Apply free transient chances if applicable
                if (freeTransientMods.Count() <= 0 || RandomU.instance.RandomDouble() > freeTransientMods.Sum((m) => m.Chance))
                {
                    Program.GetComponent<ProgramAttributeTransient>().Uses++;
                }
            }
        }
        user.OnAfterActionFn?.Invoke(this);
    }

    public IEnumerator<SubAction> GetEnumerator()
    {
        return subActions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return subActions.GetEnumerator();
    }

    public int CompareTo(Action other)
    {
        if (ActionType == other.ActionType)
        {
            return DisplayName.CompareTo(other.DisplayName);
        }
        return ActionType.CompareTo(other.ActionType);
    }
}
