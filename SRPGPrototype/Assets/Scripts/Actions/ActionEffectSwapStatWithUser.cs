using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSwapStatWithUser : ActionEffect
{
    [SerializeField] private Stats.StatName targetStat = Stats.StatName.HP;
    [SerializeField] private Stats.StatName userStat = Stats.StatName.AP;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        int temp = target.GetStat(targetStat);
        target.SetStat(targetStat, user.GetStat(userStat));
        user.SetStat(userStat, temp);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null && target.GetStat(targetStat) != user.GetStat(userStat);
    }
}
