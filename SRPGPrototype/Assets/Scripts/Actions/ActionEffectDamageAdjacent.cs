using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ActionEffectDamageAdjacent : ActionEffectDamage
{
    [SerializeField] private RangePatternGeneratorDirectional.Directions directions = RangePatternGeneratorDirectional.Directions.Horizontal;
    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };
    [SerializeField] private int modifier = 1;
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        bool ValidPos(Vector2Int p) => grid.IsLegal(p) && !grid.IsEmpty(p) && teams.Contains(grid.Get(p).UnitTeam);
        if (directions.HasFlag(RangePatternGeneratorDirectional.Directions.Both))
            return user.Pos.AdjacentBoth().Count(ValidPos) * modifier;
        if (directions.HasFlag(RangePatternGeneratorDirectional.Directions.Horizontal))
            return user.Pos.Adjacent().Count(ValidPos) * modifier;
        if (directions.HasFlag(RangePatternGeneratorDirectional.Directions.Diagonal))
            return user.Pos.AdjacentDiagonal().Count(ValidPos) * modifier;
        return 0;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
