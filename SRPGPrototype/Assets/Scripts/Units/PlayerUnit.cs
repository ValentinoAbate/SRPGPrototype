using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnit : Unit
{
    public override Team UnitTeam => Team.Player;
    public override Interference InterferenceLevel => Interference.None;

    #region Stats

    public override int MaxHP { get => Stats.MaxHP; set => Stats.MaxHP = value; }
    public override int HP { get => Stats.HP; protected set { Stats.HP = value; UI.Hp = value; } }
    public override int MaxAP { get => Stats.MaxAP; set => Stats.MaxAP = value; }
    public override int AP { get => ap; set { ap = value; UI.AP = value; } }
    private int ap = 0;
    public override int Repair { get => Stats.Repair; set => Stats.Repair = value; }
    public override CenterStat Power => Stats.Power;
    public override CenterStat Speed => Stats.Speed;
    public override CenterStat Break => Stats.Break;

    #endregion

    public override OnSubAction OnBeforeSubActionFn => Shell.OnBeforeSubAction;
    public override OnSubAction OnAfterSubActionFn => Shell.OnAfterSubAction;
    public override OnAfterAction OnAfterActionFn => Shell.OnAfterAction;
    public override OnDeath OnDeathFn => Shell.OnDeath;
    public override OnBattleStartDel OnBattleStartFn => Shell.OnBattleStart;
    public override OnPhaseStartDel OnPhaseStartFn => Shell.OnPhaseStart;

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public override string Description => description;
    [SerializeField] [TextArea(1,2)] private string description = string.Empty;

    public bool IsMain { get; set; } = false;

    public override Shell Shell => PersistantData.main.inventory.EquippedShell;

    private Stats Stats => Shell.Stats;

    public override IReadOnlyList<Action> Actions => Shell.Actions;
    public override IReadOnlyList<ModifierActionDamage> IncomingDamageModifiers => Shell.IncomingDamageModifiers;

    private int unitIndex = 0;
    public int UnitIndex
    { 
        get => unitIndex;
        set
        {
            unitIndex = value;
            UI.SetNumberText((value + 1).ToString());
        }
    }

    public override UnitUI UI => unitUI;
    [SerializeField] private UnitUI unitUI;

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
        Break.Value = 0;
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
            Debug.Log("Break: " + Break.Value.ToString());
            Debug.Log("Repair: " + Repair.ToString());
        }
    }

    public override void DoRepair()
    {
        Shell.Stats.DoRepair();
        UI.Hp = Shell.Stats.HP;
    }

    // Reset uses per turn
    public override IEnumerator OnPhaseStart(BattleGrid grid)
    {
        OnPhaseStartFn?.Invoke(grid, this);
        foreach (var action in Actions)
            action.ResetUses(Action.Trigger.TurnStart);
        yield break;
    }

    public override IEnumerator OnBattleEnd(EncounterManager manager)
    {
        foreach (var action in Actions)
            action.ResetUses(Action.Trigger.EncounterStart);
        DoRepair();
        yield break;
    }
}
