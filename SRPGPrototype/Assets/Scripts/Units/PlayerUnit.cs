using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUnit : Unit
{
    public override Team UnitTeam => Team.Player;
    public override Jamming InterferenceLevel => Jamming.None;
    public override Priority PriorityLevel => Priority.Normal;
    public override Tags UnitTags => Tags.None;

    #region Stats

    public override bool Dead => Shell == null || base.Dead;
    public override int MaxHP { get => Stats.MaxHP; set => Stats.MaxHP = value; }
    public override int HP { get => Stats.HP; protected set { Stats.HP = value; UI.Hp = value; } }
    public override int MaxAP { get => Stats.MaxAP; set => Stats.MaxAP = value; }
    public override int AP { get => ap; set { ap = value; UI.AP = value; } }
    private int ap = 0;
    public override int Repair { get => Stats.Repair; set => Stats.Repair = value; }
    public override int BaseRepair => Stats.BaseRepair;
    public override CenterStat Power => Stats.Power;
    public override CenterStat Speed => Stats.Speed;
    public override CenterStat Break => Stats.Break;

    #endregion

    public override OnSubAction OnBeforeSubActionFn => Shell.OnBeforeSubAction;
    public override OnSubAction OnAfterSubActionFn => Shell.OnAfterSubAction;
    public override OnAfterAction OnAfterActionFn => Shell.OnAfterAction;
    public override OnDeath OnDeathFn => Shell.OnDeath;
    public override OnDamaged OnDamagedFn => Shell.OnDamaged;
    public override IncomingDamageMod IncomingDamageMods => Shell.IncomingDamageMods;
    public override OnBattleStartDel OnBattleStartFn => Shell.OnBattleStart;
    public override OnPhaseStartDel OnPhaseStartFn => Shell.OnPhaseStart;

    public override string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public override string Description => description;
    [SerializeField] [TextArea(1,2)] private string description = string.Empty;

    public bool IsMain { get; set; } = false;

    public override Shell Shell => PersistantData.main.inventory.EquippedShell;

    private Stats Stats => Shell.Stats;

    public override IEnumerable<Action> Actions => Shell.Actions;

    public override UnitUI UI => unitUI;
    [SerializeField] private UnitUI unitUI;

    public virtual int LinkOutThreshold => Shell.LinkOutThreshold;

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

    public override void DoRepair()
    {
        Shell.Stats.DoRepair();
        UI.Hp = Shell.Stats.HP;
    }

    public override Coroutine OnBattleEnd(EncounterManager manager)
    {
        foreach (var action in Actions)
            action.ResetUses(Action.Trigger.EncounterStart);
        DoRepair();
        return null;
    }
}
