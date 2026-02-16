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

    public override OnSubAction OnBeforeSubActionFn { get; set; }
    public override OnSubAction OnAfterSubActionFn { get; set; }
    public override OnAfterAction OnAfterActionFn { get; set; }
    public override OnDeath OnDeathFn { get; set; }
    public override OnDamaged OnDamagedFn { get; set; }
    public override IncomingDamageMod IncomingDamageMods { get; set; }
    public override OnRepositioned OnRepositionedFn { get; set; }
    public override OnRepositioned OnRepositionOther { get; set; }
    public override OnGamble OnGambleFn { get; set; }
    public override System.Action<BattleGrid, Unit> OnSpawned { get; set; }
    public override System.Action<BattleGrid, Unit> OnRemoved { get; set; }
    public override OnBattleStartDel OnBattleStartFn { get; set; }
    public override OnPhaseStartDel OnPhaseStartFn { get; set; }
    public override OnPhaseEndDel OnPhaseEndFn { get; set; }

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
    [SerializeField] private Action[] contextActions;

    public override UnitUI UI => unitUI;
    [SerializeField] private UnitUI unitUI;

    public override CenterStat Power { get; } = new CenterStat();
    public override CenterStat Speed { get; } = new CenterStat();
    public override CenterStat Break { get; } = new CenterStat();

    public AIComponent AI { get; private set; }

    [SerializeField] private ProgramEffectAddAbility[] abilities;

    protected override void Initialize()
    {
        base.Initialize();
        AI = GetComponent<AIComponent>();
        if (AI != null)
        {
            AI.Initialize(this);
        }
        ResetStats();
        for (int i = 0; i < contextActions.Length; i++)
        {
            contextActions[i] = contextActions[i].Validate(ActionTransform);
        }
        foreach(var ability in abilities)
        {
            ability.ApplyEffect(this);
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

    public override IReadOnlyCollection<Action> GetContextualActions(Unit user, BattleGrid grid)
    {
        return contextActions.Length > 0 && grid.IsAdjacent(this, user) ? contextActions : System.Array.Empty<Action>();
    }

    private const int aiId = 0;
    private const int abilityInd = 1;

    public override SaveManager.UnitData Save()
    {
        var data = base.Save();
        if (AI != null && AI.CanSave)
        {
            data.AddData(aiId, AI.Save());
        }
        if (abilities.Length > 0)
        {
            List<string> effectData = null;
            for (int i = 0; i < abilities.Length; i++)
            {
                var ability = abilities[i];
                if (ability.CanSave(true))
                {
                    effectData ??= new List<string>(abilities.Length - i);
                    effectData.Add(ability.Save(true));
                }
            }
            if (effectData != null)
            {
                data.AddData(abilityInd, effectData);
            }
        }
        return data;
    }

    public override void Load(SaveManager.UnitData data, SaveManager.Loader loader)
    {
        base.Load(data, loader);
        foreach(var d in data.d)
        {
            if(d.t == aiId)
            {
                if (d.Count > 0)
                {
                    AI.Load(d.d[0]);
                }
            }
            else if(d.t == abilityInd)
            {
                if (d.Count > 0)
                {
                    int index = 0;
                    foreach (var ability in abilities)
                    {
                        if (ability.CanSave(true))
                        {
                            ability.Load(d[index++], true, this);
                            if (index >= d.Count)
                                break;
                        }
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    public void AttachAbilities()
    {
        abilities = GetComponents<ProgramEffectAddAbility>();
    }
#endif
}
