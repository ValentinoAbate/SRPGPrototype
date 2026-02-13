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

    public override OnSubAction OnBeforeSubActionFn { get => CompileData.onBeforeSubAction; set { } }

    public override OnSubAction OnAfterSubActionFn { get => CompileData.onAfterSubAction; set { } }

    public override OnAfterAction OnAfterActionFn { get => CompileData.onAfterAction; set { } }

    public override OnDeath OnDeathFn { get => CompileData.onDeath; set { } }

    public override OnDamaged OnDamagedFn { get => CompileData.onDamaged; set { } }

    public override OnRepositioned OnRepositionedFn { get => CompileData.onRepositioned; set { } }
    public override OnRepositioned OnRepositionOther { get => CompileData.onRepositionOther; set { } }
    public override OnGamble OnGambleFn { get => CompileData.onGamble; set { } }

    public override IncomingDamageMod IncomingDamageMods { get => CompileData.incomingDamageMods; set { } }

    public override OnBattleStartDel OnBattleStartFn { get => CompileData.onBattleStart; set { } }

    public override OnPhaseStartDel OnPhaseStartFn { get => CompileData.onPhaseStart; set { } }

    public override OnPhaseEndDel OnPhaseEndFn { get => CompileData.onPhaseEnd; set { } }

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
