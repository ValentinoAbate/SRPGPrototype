using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageProgramNumber : ActionEffectDamage
{
    public ProgramNumber damage;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        return damage.Value(action.Program);
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
