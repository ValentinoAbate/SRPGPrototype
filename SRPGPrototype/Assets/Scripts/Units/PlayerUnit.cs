using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnit : Unit
{
    public override Team UnitTeam => Team.Player;

    #region Stats

    public override int MaxHP { get => Stats.MaxHP; set => Stats.MaxHP = value; }
    public override int HP { get => Stats.HP; protected set { Stats.HP = value; unitUI.Hp = value; } }
    public override int MaxAP { get => Stats.MaxAP; set => Stats.MaxAP = value; }
    public override int AP { get => ap; set { ap = value; unitUI.AP = value; } }
    private int ap = 0;
    public override int Repair { get => Stats.Repair; set => Stats.Repair = value; }
    public override CenterStat Power => Stats.Power;
    public override CenterStat Speed => Stats.Speed;
    public override CenterStat Defense => Stats.Defense;

    #endregion

    public override OnAfterSubAction OnAfterSubActionFn => Shell.OnAfterSubAction;
    public override OnAfterAction OnAfterActionFn => Shell.OnAfterAction;
    public override OnDeath OnDeathFn => Shell.OnDeath;
    public override OnBattleStartDel OnBattleStartFn => Shell.OnBattleStart;

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public override string Description => description;
    [SerializeField] [TextArea(1,2)] private string description = string.Empty;

    public override Shell Shell => PersistantData.main.inventory.EquippedShell;

    private Stats Stats => Shell.Stats;

    public override List<Action> Actions => Shell.Actions.ToList();

    public UnitUI unitUI;

    public override void ResetStats()
    {
        AP = MaxAP;
        if (HP <= 0)
            HP = MaxHP;
        // Make sure UI gets updated
        HP = HP;
        // Reset center stats
        Power.Value = 0;
        Speed.Value = 0;
        Defense.Value = 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        ResetStats();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Power: " + Power.Value.ToString());
            Debug.Log("Speed: " + Speed.Value.ToString());
            Debug.Log("Defense: " + Defense.Value.ToString());
            Debug.Log("Repair: " + Repair.ToString());
        }
    }

    public override void DoRepair()
    {
        Shell.Stats.DoRepair();
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
        DoRepair();
        yield break;
    }
}
