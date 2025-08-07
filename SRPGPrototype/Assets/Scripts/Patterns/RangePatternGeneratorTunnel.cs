using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorTunnel : RangePatternGenerator
{
    [SerializeField] private AdjacencyDirections directions;
    [SerializeField] private bool allowAdjacentPositions = true;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        foreach(var direction in directions.GetDirectionVectors())
        {
            var pos = grid.FirstEmptyPositionInDirection(userPos, direction);
            if (pos != BattleGrid.OutOfBounds && (allowAdjacentPositions || pos != userPos + direction))
                yield return pos;
        }
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        foreach (var direction in directions.GetDirectionVectors())
        {
            var pos = grid.FirstEmptyPositionInDirection(targetPos, direction);
            if (pos != BattleGrid.OutOfBounds && (allowAdjacentPositions || pos != targetPos + direction))
                yield return pos;
        }
    }

    public override int MaxDistance(BattleGrid grid)
    {
        if(directions.HasFlag(AdjacencyDirections.Diagonal))
            return grid.MaxGridDistance;
        if (directions.HasFlag(AdjacencyDirections.HorizontalVertical))
            return Mathf.Max(grid.Dimensions.x, grid.Dimensions.y) - 1;
        return 0;
    }
}
