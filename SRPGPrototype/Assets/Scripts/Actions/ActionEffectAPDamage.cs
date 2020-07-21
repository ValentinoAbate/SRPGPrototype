using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectAPDamage : ActionEffect
{
    public ProgramNumber damage = new ProgramNumber();
    private int damageNumber;

    public override void Initialize(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        damageNumber = damage.Value(action.Program);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        target.AP -= damageNumber;
        Debug.Log(target.DisplayName + " takes " + damageNumber.ToString() + " AP damage and is now at " + target.AP + " hp");
    }
}
