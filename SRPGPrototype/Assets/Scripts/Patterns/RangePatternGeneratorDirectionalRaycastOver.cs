using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorDirectionalRaycastOver : RangePatternGenerator
{
    [SerializeField] private AdjacencyDirections directions;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        foreach(var direction in directions.GetDirectionVectors())
        {
            var unit = grid.Raycast(userPos, direction);
            if (unit != null && grid.IsLegal(unit.Pos + direction))
                yield return unit.Pos + direction;
        }
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        if (!grid.IsLegal(targetPos))
            yield break;
        foreach (var direction in directions.GetDirectionVectors())
        {
            var modPos = targetPos + direction;
            if (grid.IsEmpty(modPos))
                continue;
            foreach(var pos in grid.PositionsUntilRaycastHit(modPos, direction))
            {
                yield return pos;
            }
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
