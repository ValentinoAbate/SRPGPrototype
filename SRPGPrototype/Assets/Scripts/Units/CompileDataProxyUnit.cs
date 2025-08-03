using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompileDataProxyUnit : Unit
{
    public override Team UnitTeam => Team.Player;

    public override Jamming InterferenceLevel => throw new System.NotImplementedException();

    public override Priority PriorityLevel => Priority.Normal;

    public override Tags UnitTags => Tags.None;

    public override int MaxHP { get => CompileData.stats.MaxHP; set { } }
    public override int HP { get; protected set; }
    public override int MaxAP { get => CompileData.stats.MaxAP; set { } }
    public override int AP { get => CompileData.stats.MaxAP; set { } }
    public override int Repair { get => CompileData.stats.Repair; set { } }

    public override int BaseRepair => CompileData.stats.Repair;

    public override CenterStat Power => CompileData.stats.Power;

    public override CenterStat Speed => CompileData.stats.Speed;

    public override CenterStat Break => CompileData.stats.Break;

    public override OnSubAction OnBeforeSubActionFn => CompileData.onBeforeSubAction;

    public override OnSubAction OnAfterSubActionFn => CompileData.onAfterSubAction;

    public override OnAfterAction OnAfterActionFn => CompileData.onAfterAction;

    public override OnDeath OnDeathFn => CompileData.onDeath;

    public override OnDamaged OnDamagedFn => CompileData.onDamaged;

    public override IncomingDamageMod IncomingDamageMods => CompileData.incomingDamageMods;

    public override OnBattleStartDel OnBattleStartFn => CompileData.onBattleStart;

    public override OnPhaseStartDel OnPhaseStartFn => CompileData.onPhaseStart;

    public override string DisplayName => Shell.DisplayName;

    public override string Description => string.Empty;

    public override UnitUI UI => null;

    public override Shell Shell => null;
    public override IEnumerable<Action> Actions => CompileData.actions;

    public Shell.CompileData CompileData { get; private set; } = new Shell.CompileData();

    public void UpdateData(int currentHp, Shell.CompileData compileData)
    {
        HP = currentHp;
        CompileData = compileData;
    }
}
