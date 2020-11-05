using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            new List<SubAction>(GetComponentsInChildren<SubAction>());
    }

    public Action Validate(Transform parent)
    {
        if (transform.IsChildOf(parent))
            return this;
        return Instantiate(gameObject, parent).GetComponent<Action>();
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
        if (Program == null)
            return;
        if(Program.attributes.HasFlag(Program.Attributes.Transient))
        {
            Program.GetComponent<ProgramAttributeTransient>().Uses++;
        }
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
