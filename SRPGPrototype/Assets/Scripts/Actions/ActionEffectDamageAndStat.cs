using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageAndStat : ActionEffectDamageBasic
{
    [SerializeField] private Stats.StatName gainStat;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        int damage = DealDamage(grid, action, sub, user, target, targetData);
        target.ModifyStat(grid, gainStat, damage, user);
    }
}
