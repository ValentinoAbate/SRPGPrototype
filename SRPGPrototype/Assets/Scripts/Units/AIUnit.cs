using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIUnit : Unit
{
    public override Team UnitTeam => team;
    [SerializeField] private Team team = Team.None;
    public override Jamming InterferenceLevel => interferenceLevel;
    [SerializeField] private Jamming interferenceLevel = Jamming.None;

    public override Priority PriorityLevel => priorityLevel;
    [SerializeField] private Priority priorityLevel = Priority.Normal;

    public override Tags UnitTags => tags;
    [SerializeField] private Tags tags;

    public override int MaxHP { get => maxHP; set { maxHP = value; HP = Mathf.Min(HP, value); } }
    [SerializeField] private int maxHP = 3;

    public override int HP { get => hp; protected set { hp = value; UI.Hp = value; } }
    private int hp = 0;

    public override int MaxAP { get => maxAP; set { maxAP = value; AP = Mathf.Min(AP, value); } }
    [SerializeField] private int maxAP = 3;

    public override int AP { get => ap; set { ap = value; UI.AP = value; } }
    private int ap = 0;

    public override int Repair { get; set; }
    public override int BaseRepair { get; }

    public override OnSubAction OnBeforeSubActionFn { get; }
    public override OnSubAction OnAfterSubActionFn { get; }
    public override OnAfterAction OnAfterActionFn { get; }
    public override OnDeath OnDeathFn { get => onDeathFn; }
    private OnDeath onDeathFn = null;
    public override OnDamaged OnDamagedFn { get => onDamagedFn; }
    private OnDamaged onDamagedFn = null;
    public override IncomingDamageMod IncomingDamageMods => incomingDamageMods;
    private IncomingDamageMod incomingDamageMods = null;
    public override OnBattleStartDel OnBattleStartFn { get; }
    public override OnPhaseStartDel OnPhaseStartFn { get; }
    public override OnPhaseEndDel OnPhaseEndFn => onPhaseEnd;
    private OnPhaseEndDel onPhaseEnd;

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public override string Description => description;
    [SerializeField] [TextArea(1, 2)] private string description = string.Empty;

    public override Shell Shell => null;

    public override IEnumerable<Action> Actions
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

    public Transform ActionTransform => actionTransform;
    [SerializeField] private Transform actionTransform = null;

    public override UnitUI UI => unitUI;
    [SerializeField] private UnitUI unitUI;

    public override CenterStat Power { get; } = new CenterStat();
    public override CenterStat Speed { get; } = new CenterStat();
    public override CenterStat Break { get; } = new CenterStat();

    public abstract AIComponent<AIUnit> AI { get; }

    [SerializeField] private ProgramEffectAddIncomingDamageModifierAbility[] incomingDamageModifierAbilities;
    [SerializeField] private ProgramEffectAddOnPhaseEndAbility[] onPhaseEndAbilities;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
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
        var onDamagedEffects = GetComponents<ProgramEffectAddOnDamagedAbility>();
        foreach (var effect in onDamagedEffects)
        {
            onDamagedFn += effect.Ability;
        }
        foreach(var effect in incomingDamageModifierAbilities)
        {
            incomingDamageMods += effect.Ability;
        }
        foreach(var effect in onPhaseEndAbilities)
        {
            onPhaseEnd += effect.Ability;
        }
    }

    public Coroutine DoTurn(BattleGrid grid)
    {
        if (AI == null)
            return null;
        return StartCoroutine(AI.DoTurn(grid, this));
    }

    public override Coroutine OnBattleEnd(EncounterManager manager)
    {
        if (Dead)
        {
            return null;
        }
        var progDrops = GetComponentsInChildren<DropComponent<Program>>(true);
        foreach (var drop in progDrops)
            manager.GenerateProgramLoot += drop.GenerateDrop;
        var shellDrops = GetComponentsInChildren<DropComponent<Shell>>(true);
        foreach (var drop in shellDrops)
            manager.GenerateShellLoot += drop.GenerateDrop;
        var moneyRewards = GetComponentsInChildren<MoneyRewardComponent>(true);
        foreach (var reward in moneyRewards)
            manager.GenerateMoneyLoot += reward.GenerateMoneyData;
        return null;
    }
}
