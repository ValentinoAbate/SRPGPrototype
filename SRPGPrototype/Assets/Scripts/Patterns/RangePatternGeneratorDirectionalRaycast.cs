using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorDirectionalRaycast : RangePatternGenerator
{
    [SerializeField] private AdjacencyDirections directions;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        foreach(var direction in directions.GetDirectionVectors())
        {
            var unit = grid.Raycast(userPos, direction);
            if (unit != null)
                yield return unit.Pos;
        }
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        foreach (var direction in directions.GetDirectionVectors())
        {
            foreach(var pos in grid.PositionsUntilRaycastHit(targetPos, direction))
            {
                yield return pos;
            }
        }
    }

    public override int MaxDistance(BattleGrid grid)
    {
        if(directions.HasFlag(AdjacencyDirections.Diagonal))
            return grid.MaxGridDistance;
        if (directions.HasFlag(AdjacencyDirections.Horizontal))
            return Mathf.Max(grid.Dimensions.x, grid.Dimensions.y) - 1;
        return 0;
    }
}
