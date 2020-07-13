using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamage : ActionEffect
{
    public int damage = 1;

    public override void ApplyEffect(BattleGrid grid, Combatant user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        target.Damage(damage);
        Debug.Log(target.DisplayName + " takes " + damage.ToString() + " damage and is now at " + target.HP + " hp");
    }
}
