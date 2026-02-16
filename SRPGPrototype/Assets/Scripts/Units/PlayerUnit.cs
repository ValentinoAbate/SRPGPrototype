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

    public override OnSubAction OnBeforeSubActionFn { get => Shell.OnBeforeSubAction; set { } }
    public override OnSubAction OnAfterSubActionFn { get => Shell.OnAfterSubAction; set { } }
    public override OnAfterAction OnAfterActionFn { get => Shell.OnAfterAction; set { } }
    public override OnDeath OnDeathFn { get => Shell.OnDeath; set { } }
    public override OnDamaged OnDamagedFn { get => Shell.OnDamaged; set { } }
    public override IncomingDamageMod IncomingDamageMods { get => Shell.IncomingDamageMods; set { } }
    public override OnRepositioned OnRepositionedFn { get => Shell.OnRepositioned; set { } }
    public override OnRepositioned OnRepositionOther { get => Shell.OnRepositionOther; set { } }
    public override OnGamble OnGambleFn { get => Shell.OnGamble; set { } }
    public override System.Action<BattleGrid, Unit> OnSpawned { get => Shell.OnSpawned; set { } }
    public override System.Action<BattleGrid, Unit> OnRemoved { get => Shell.OnRemoved; set { } }
    public override OnBattleStartDel OnBattleStartFn { get => Shell.OnBattleStart; set { } }
    public override OnPhaseStartDel OnPhaseStartFn { get => Shell.OnPhaseStart; set { } }
    public override OnPhaseEndDel OnPhaseEndFn { get => Shell.OnPhaseEnd; set { } }

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

    private const int pdId = 0;

    public override SaveManager.UnitData Save()
    {
        var data = base.Save();
        data.AddData(pdId, SaveArgs());
        return data;
    }

    public override void Load(SaveManager.UnitData data, SaveManager.Loader loader)
    {
        // Load Shell
        foreach (var d in data.d)
        {
            if (d.t == pdId && d.Count > 0)
            {
                var stack = new Queue<string>(d);
                LoadArgs(stack, loader);
                break;
            }
        }
        base.Load(data, loader);
        loader.LoadUnloadedShell(Shell.Id, true, this);
    }

    protected virtual List<string> SaveArgs()
    {
        var args = new List<string>();
        args.Add(BoolUtils.ToStringInt(IsMain));
        return args;
    }

    protected virtual void LoadArgs(Queue<string> args, SaveManager.Loader loader)
    {
        if (args.Count <= 0)
            return;
        IsMain = BoolUtils.FromStringInt(args.Dequeue());
    }
}
