using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamage : ActionEffect
{
    public ProgramNumber damage = new ProgramNumber();

    public override void ApplyEffect(BattleGrid grid, Action action, Combatant user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        int damageNumber = damage.Value(action.Program);
        target.Damage(damageNumber);
        Debug.Log(target.DisplayName + " takes " + damageNumber.ToString() + " damage and is now at " + target.HP + " hp");
    }
}
