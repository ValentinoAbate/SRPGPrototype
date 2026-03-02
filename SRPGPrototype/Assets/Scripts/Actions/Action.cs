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
        Special
    }

    public enum Trigger
    { 
        Never,
        TurnStart,
        EncounterStart,
    }

    public Program Program { get; set; }

    public Type ActionType
    {
        get => type;
        set => type = value;
    }

    [SerializeField] Type type = Type.Weapon;

    public int APCost 
    {
        get
        {
            if (SlowdownInterval == 0)
                return baseAp;
            int usage = TimesUsed - FreeUses;
            if (SlowdownReset == Trigger.TurnStart)
                usage = TimesUsedThisTurn - FreeUsesThisTurn;
            else if (SlowdownReset == Trigger.EncounterStart)
                usage = TimesUsedThisBattle - FreeUsesThisBattle;
            return baseAp + Slowdown * (usage / SlowdownInterval);
        }
    }
    public int BaseAPCost
    {
        get => baseAp;
        set => baseAp = value;
    }
    [SerializeField] private int baseAp = 1;

    public int Slowdown
    {
        get => slowdown;
        set => slowdown = value;
    }

    [SerializeField] private int slowdown = 1;

    public int SlowdownInterval
    {
        get => slowdownInterval;
        set => slowdownInterval = value;
    }

    [SerializeField] private int slowdownInterval = 1;

    public Trigger SlowdownReset
    {
        get => slowdownReset;
        set => slowdownReset = value;
    }
    [SerializeField] private Trigger slowdownReset = Trigger.TurnStart;

    public int TimesUsed { get; private set; } = 0;
    public int FreeUses { get; private set; } = 0;

    public int TimesUsedThisBattle { get; private set; } = 0;
    public int FreeUsesThisBattle { get; private set; } = 0;

    public int TimesUsedThisTurn { get; private set; } = 0;
    public int FreeUsesThisTurn { get; private set; } = 0;

    public string DisplayName
    {
        get => displayName;
        set => displayName = value;
    }
    [SerializeField] private string displayName = string.Empty;

    public string GetDescription(Unit user, BattleGrid grid) => TextMacros.ApplyActionTextMacros(description, this, user, grid);
    public string SetDescription(string newDescription) => description = newDescription;
    [SerializeField] [TextArea(1, 3)] private string description = string.Empty;

    public string ActionTypeText
    {
        get
        {
            return $"{ActionType} ({SubActionTypeText})";
        }
    }

    private string SubActionTypeText
    {
        get
        {
            if (SubActions.Count <= 0)
                return string.Empty;
            if (SubActions.Count == 1)
                return SubActions[0].SubTypeText;
            return string.Join(" / ", SubActions.Select((s) => s.SubTypeText).Where((s) => !string.IsNullOrEmpty(s)));
        }
    }

    public string ModifierText
    {
        get
        {
            return Program != null ? Program.ModifiedByText : string.Empty;
        }
    }

    public string FullName
    {
        get
        {
            var modText = ModifierText;
            if (string.IsNullOrEmpty(modText))
            {
                return $"{DisplayName} - {ActionTypeText}";
            }
            return $"{DisplayName} {modText} - {ActionTypeText}";
        }
    }

    public IReadOnlyList<SubAction> SubActions => subActions;
    public void SetSubActions(IReadOnlyList<SubAction> newSubActions)
    {
        subActions = new SubAction[newSubActions.Count];
        for (int i = 0; i < newSubActions.Count; i++)
        {
            subActions[i] = newSubActions[i];
        }
    }
    [SerializeField] private SubAction[] subActions;

    public IEnumerable<Vector2Int> GetRange(BattleGrid grid, Vector2Int origin, Unit user, int subActionIndex = 0)
    {
        if (subActionIndex < 0 || subActionIndex >= SubActions.Count)
            return Array.Empty<Vector2Int>();
        return subActions[subActionIndex].Range.GetPositions(grid, origin, user);
    }

    public IEnumerable<Vector2Int> GetTargets(BattleGrid grid, Unit user, Vector2Int targetPos, int subActionIndex = 0)
    {
        if (subActionIndex < 0 || subActionIndex >= SubActions.Count)
            return Array.Empty<Vector2Int>();
        return subActions[subActionIndex].targetPattern.Target(grid, user, targetPos);
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

    public int MaxUses(int apBudget)
    {
        int cost = APCost;
        if (cost > apBudget)
            return 0;
        int uses = 1;
        while (uses < 100)
        {
            cost += APCostAfterXUses(uses);
            if (cost > apBudget)
                break;
            ++uses;
        }
        return uses;
    }

    public int APCostAfterXUses(int uses)
    {
        if (SlowdownInterval == 0)
            return baseAp;
        int baseUsage = TimesUsed;
        if (SlowdownReset == Trigger.TurnStart)
            baseUsage = TimesUsedThisTurn;
        else if (SlowdownReset == Trigger.EncounterStart)
            baseUsage = TimesUsedThisBattle;
        return baseAp + Slowdown * ((baseUsage + uses) / SlowdownInterval);
    }

    public void UseAll(BattleGrid grid, Unit user, Vector2Int targetPos, bool applyAPCost = true)
    {
        var lastTargets = new List<Unit>();
        foreach (var sub in SubActions)
        {
            sub.Use(grid, this, user, targetPos, lastTargets, out var targets);
            lastTargets.Clear();
            lastTargets.AddRange(targets);
        }
        FinishAction(grid, user, applyAPCost);
    }

    public void FinishAction(BattleGrid grid, Unit user, bool applyAPCost = true)
    {
        int cost = 0;
        if(applyAPCost)
        {
            cost = APCost;
            user.AP -= Mathf.Max(cost, 0);
        }
        ++TimesUsed;
        ++TimesUsedThisBattle;
        ++TimesUsedThisTurn;
        if(Program != null)
        {
            var noSlowdownMods = Program.ModifiedByType<ModifierActionNoSlowdownChance>();
            if (noSlowdownMods.Any() && RandomU.instance.RandomDouble() < noSlowdownMods.Sum((m) => m.Chance))
            {
                GrantFreeUse();
            }
            if(Program.attributes.HasFlag(Program.Attributes.Transient))
            {
                var freeTransientMods = Program.ModifiedByType<ModifierActionFreeTransientChance>();
                // Apply free transient chances if applicable
                if (!freeTransientMods.Any() || RandomU.instance.RandomDouble() > freeTransientMods.Sum((m) => m.Chance))
                {
                    Program.GetComponent<ProgramAttributeTransient>().Use(grid, user);
                }
            }
        }
        user.OnAfterActionFn?.Invoke(grid, this, user, cost);
    }

    public void GrantFreeUse()
    {
        ++FreeUses;
        ++FreeUsesThisBattle;
        ++FreeUsesThisTurn;
    }

    public IEnumerator<SubAction> GetEnumerator()
    {
        return SubActions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return SubActions.GetEnumerator();
    }

    public int CompareTo(Action other)
    {
        if (ActionType == other.ActionType)
        {
            return DisplayName.CompareTo(other.DisplayName);
        }
        return ActionType.CompareTo(other.ActionType);
    }

    private const char separator = '_';

    public string Save(bool isBattle)
    {
        var ret = new System.Text.StringBuilder();
        ret.Append(TimesUsed);
        ret.Append(separator);
        ret.Append(FreeUses);
        ret.Append(separator);
        if (isBattle)
        {
            ret.Append(TimesUsedThisBattle);
            ret.Append(separator);
            ret.Append(FreeUsesThisBattle);
            ret.Append(separator);
            ret.Append(TimesUsedThisTurn);
            ret.Append(separator);
            ret.Append(FreeUsesThisTurn);
            ret.Append(separator);
        }
        foreach(var sub in SubActions)
        {
            if (sub.CanSave(isBattle))
            {
                ret.Append(sub.Save(isBattle));
                ret.Append(separator);
            }
        }
        ret.Remove(ret.Length - 1, 1);
        return ret.ToString();
    }

    public void Load(string data, bool isBattle)
    {
        var args = data.Split(separator);
        int ind = 0;
        try
        {
            if(int.TryParse(args[ind++], out int timesUsed))
            {
                TimesUsed = timesUsed;
            }
            if(int.TryParse(args[ind++], out int freeUses))
            {
                FreeUses = freeUses;
            }
            if (isBattle)
            {
                if (int.TryParse(args[ind++], out int timesUsedThisBattle))
                {
                    TimesUsedThisBattle = timesUsedThisBattle;
                }
                if (int.TryParse(args[ind++], out int freeUsesThisBattle))
                {
                    FreeUsesThisBattle = freeUsesThisBattle;
                }
                if (int.TryParse(args[ind++], out int timesUsedThisTurn))
                {
                    TimesUsedThisTurn = timesUsedThisTurn;
                }
                if (int.TryParse(args[ind++], out int freeUsesThisTurn))
                {
                    FreeUsesThisTurn = freeUsesThisTurn;
                }
            }
        }
        catch
        {
            return;
        }
        int subInd = 0;
        while(ind < args.Length && subInd < subActions.Length)
        {
            var sub = subActions[subInd++];
            if (sub.CanSave(isBattle))
            {
                sub.Load(args[ind++], isBattle);
            }
        }
    }
}
