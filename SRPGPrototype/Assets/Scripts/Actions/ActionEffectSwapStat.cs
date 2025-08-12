using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSwapStat : ActionEffect
{
    [SerializeField] private Stats.StatName stat1 = Stats.StatName.HP;
    [SerializeField] private Stats.StatName stat2 = Stats.StatName.AP;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        int temp = target.GetStat(stat1);
        target.SetStat(stat1, target.GetStat(stat2));
        target.SetStat(stat2, temp);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null;
    }
}
