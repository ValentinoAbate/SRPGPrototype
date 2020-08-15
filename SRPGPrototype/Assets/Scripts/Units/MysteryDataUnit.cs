using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryDataUnit : Unit, IEncounterUnit
{
    public override int MaxHP { get => maxHP; set { maxHP = value; HP = Mathf.Min(HP, value); } }
    [SerializeField] private int maxHP = 3;

    public override int HP { get => hp; protected set { hp = value; unitUI.Hp = value; } }
    private int hp = 0;

    public override int MaxAP { get => 0; set { } }

    public override int AP { get => 0; set { } }
    public override int Repair { get; set; }
    public override CenterStat Power { get; } = new CenterStat();
    public override CenterStat Speed { get; } = new CenterStat();
    public override CenterStat Defense { get; } = new CenterStat();

    public override OnAfterSubAction OnAfterSubActionFn { get; }
    public override OnDeath OnDeathFn { get => onDeathFn; }
    private OnDeath onDeathFn = null;

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override Team UnitTeam => Team.None;

    public override Shell Shell => throw new System.NotImplementedException();

    public override List<Action> Actions => throw new System.NotImplementedException();

    public UnitUI unitUI;

    #region IEncounterUnit implementation

    public EncounterUnitData EncounterData => encounterData;
    [SerializeField] private EncounterUnitData encounterData = null;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ResetStats();
        var onDeathEffects = GetComponents<ProgramEffectAddOnDeathAbility>();
        foreach (var effect in onDeathEffects)
        {
            onDeathFn += effect.Ability;
        }
    }

    public override IEnumerator OnBattleEnd(EncounterManager manager)
    {
        if(Dead)
            yield break;
        var progDrops = GetComponentsInChildren<DropComponent<Program>>();
        foreach (var drop in progDrops)
            manager.GenerateProgramLoot += drop.GenerateDrop;
        var shellDrops = GetComponentsInChildren<DropComponent<Shell>>();
        foreach (var drop in shellDrops)
            manager.GenerateShellLoot += drop.GenerateDrop;
    }
}
