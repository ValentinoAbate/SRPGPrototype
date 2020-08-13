using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnit : Unit
{
    public override Team UnitTeam => Team.Player;

    public override int MaxHP { get => Stats.MaxHP; set => Stats.MaxHP = value; }

    public override int HP { get => Stats.HP; protected set { Stats.HP = value; unitUI.Hp = value; } }

    public override int MaxAP { get => Stats.MaxAP; set => Stats.MaxAP = value; }

    public override int AP { get => ap; set { ap = value; unitUI.AP = value; } }
    private int ap = 0;

    public override int Repair { get => Stats.Repair; set => Stats.Repair = value; }

    public override CenterStat Power => Stats.Power;
    public override CenterStat Speed => Stats.Speed;
    public override CenterStat Defense => Stats.Defense;

    public override OnAfterSubAction OnAfterSubActionFn => Shell.AbilityOnAfterSubAction;

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public override Shell Shell => PersistantData.main.inventory.EquippedShell;

    private Stats Stats => Shell.Stats;

    public override List<Action> Actions => Shell.Actions.ToList();

    public UnitUI unitUI;

    // Start is called before the first frame update
    void Start()
    {
        AP = MaxAP;
        if (HP <= 0)
            HP = MaxHP;
        // Make sure UI gets updated
        HP = HP;
    }

    // Reset uses per turn
    public override IEnumerator OnPhaseStart()
    {
        foreach (var action in Actions)
            action.TimesUsedThisTurn = 0;
        yield break;
    }

    public override IEnumerator OnBattleEnd(EncounterManager manager)
    {
        Shell.Stats.DoRepair();
        yield break;
    }
}
