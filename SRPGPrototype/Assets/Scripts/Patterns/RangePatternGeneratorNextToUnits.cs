using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangePatternGeneratorNextToUnits : RangePatternGenerator
{
    [SerializeField] private AdjacencyDirections direction = AdjacencyDirections.HorizontalVertical;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        if (user == null)
            yield break;
        foreach (var unit in grid)
        {
            if (!UnitPredicate(grid, unit, userPos, user))
            {
                continue;
            }
            foreach (var dir in direction.GetDirectionVectors())
            {
                var pos = unit.Pos + dir;
                if (grid.IsLegalAndEmpty(pos))
                {
                    yield return pos;
                }
            }
        }
    }

    protected abstract bool UnitPredicate(BattleGrid grid, Unit unit, Vector2Int userPos, Unit user);

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        yield break; // not implementing unless needed
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.MaxGridDistance;
    }
}
