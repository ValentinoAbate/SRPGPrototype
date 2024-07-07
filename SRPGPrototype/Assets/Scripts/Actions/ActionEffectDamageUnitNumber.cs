using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageUnitNumber : ActionEffectDamage
{
    [SerializeField] private UnitNumber unitNumber;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return BasicDamage(action, user);
    }

    public override int BasicDamage(Action action, Unit user)
    {
        return unitNumber.Value(user);
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
