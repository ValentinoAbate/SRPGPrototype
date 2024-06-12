using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIUnit : Unit
{
    public override Team UnitTeam => team;
    [SerializeField] private Team team = Team.None;
    public override Interference InterferenceLevel => interferenceLevel;
    [SerializeField] private Interference interferenceLevel = Interference.None;

    public override int MaxHP { get => maxHP; set { maxHP = value; HP = Mathf.Min(HP, value); } }
    [SerializeField] private int maxHP = 3;

    public override int HP { get => hp; protected set { hp = value; unitUI.Hp = value; } }
    private int hp = 0;

    public override int MaxAP { get => maxAP; set { maxAP = value; AP = Mathf.Min(AP, value); } }
    [SerializeField] private int maxAP = 3;

    public override int AP { get => ap; set { ap = value; unitUI.AP = value; } }
    private int ap = 0;

    public override int Repair { get; set; }

    public override OnSubAction OnBeforeSubActionFn { get; }
    public override OnSubAction OnAfterSubActionFn { get; }
    public override OnAfterAction OnAfterActionFn { get; }
    public override OnDeath OnDeathFn { get => onDeathFn; }
    private OnDeath onDeathFn = null;
    public override OnBattleStartDel OnBattleStartFn { get; }
    public override OnPhaseStartDel OnPhaseStartFn { get; }

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public override string Description => description;
    [SerializeField] [TextArea(1, 2)] private string description = string.Empty;

    public override Shell Shell => throw new System.NotImplementedException();

    public override IReadOnlyList<Action> Actions
    {   
        get
        {
            if(AI == null)
            {
                return System.Array.Empty<Action>();
            }
            return AI.Actions;
        } 
    }

    public override IReadOnlyList<ModifierActionDamage> IncomingDamageModifiers => System.Array.Empty<ModifierActionDamage>();

    public Transform ActionTransform => actionTransform;
    [SerializeField] private Transform actionTransform = null;

    public UnitUI unitUI;

    public override CenterStat Power { get; } = new CenterStat();
    public override CenterStat Speed { get; } = new CenterStat();
    public override CenterStat Break { get; } = new CenterStat();

    public abstract AIComponent<AIUnit> AI { get; }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (AI != null)
        {
            AI.Initialize(this);
        }
        ResetStats();
        var onDeathEffects = GetComponents<ProgramEffectAddOnDeathAbility>();
        foreach (var effect in onDeathEffects)
        {
            onDeathFn += effect.Ability;
        }
    }

    public IEnumerator DoTurn(BattleGrid grid)
    {
        if (AI == null)
            yield break;
        yield return StartCoroutine(AI.DoTurn(grid, this));
    }

    public override IEnumerator OnPhaseStart(BattleGrid grid)
    {
        OnPhaseStartFn?.Invoke(grid, this);
        foreach (var action in Actions)
        {
            action.ResetUses(Action.Trigger.TurnStart);
        }
        yield break;
    }

    public override IEnumerator OnBattleEnd(EncounterManager manager)
    {
        if (Dead)
            yield break;
        var progDrops = GetComponentsInChildren<DropComponent<Program>>(true);
        foreach (var drop in progDrops)
            manager.GenerateProgramLoot += drop.GenerateDrop;
        var shellDrops = GetComponentsInChildren<DropComponent<Shell>>(true);
        foreach (var drop in shellDrops)
            manager.GenerateShellLoot += drop.GenerateDrop;
    }
}
