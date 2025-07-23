using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ActionEffectDamageAdjacent : ActionEffectDamage
{
    [SerializeField] private AdjacencyDirections directions = AdjacencyDirections.Horizontal;
    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };
    [SerializeField] private int modifier = 1;
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        bool ValidPos(Vector2Int p) => grid.IsLegal(p) && !grid.IsEmpty(p) && teams.Contains(grid.Get(p).UnitTeam);
        if (directions.HasFlag(AdjacencyDirections.Both))
            return user.Pos.AdjacentBoth().Count(ValidPos) * modifier;
        if (directions.HasFlag(AdjacencyDirections.Horizontal))
            return user.Pos.Adjacent().Count(ValidPos) * modifier;
        if (directions.HasFlag(AdjacencyDirections.Diagonal))
            return user.Pos.AdjacentDiagonal().Count(ValidPos) * modifier;
        return 0;
    }

    public override int BasicDamage(Action action, Unit user)
    {
        return 0;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
