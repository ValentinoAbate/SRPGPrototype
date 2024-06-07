using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageSameRowColumn : ActionEffectDamage
{
    public enum Option
    { 
        Row,
        Column,
        Either,
    }

    [SerializeField] private Option option;
    [SerializeField] private int sameNumber;
    [SerializeField] private int differentNumber;

    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return 0;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return option switch
        {
            Option.Row => user.Pos.y == target.Pos.y ? sameNumber : differentNumber,
            Option.Column => user.Pos.x == target.Pos.x ? sameNumber : differentNumber,
            Option.Either => user.Pos.y == target.Pos.y || user.Pos.x == target.Pos.x ? sameNumber : differentNumber,
            _ => throw new System.NotImplementedException("Invalid Option"),
        };
    }

    public override int BasicDamage(Action action, Unit user)
    {
        return 0;
    }
}
