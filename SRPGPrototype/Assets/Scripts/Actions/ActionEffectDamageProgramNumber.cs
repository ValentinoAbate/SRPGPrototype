using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageProgramNumber : ActionEffectDamage
{
    public ProgramNumber damage;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return BasicDamage(action, user);
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }

    public override int BasicDamage(Action action, Unit user)
    {
        return damage.Value(action.Program);
    }
}
