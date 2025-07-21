using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorDirectional : RangePatternGenerator
{
    [System.Flags]
    public enum Directions
    { 
        None = 0,
        Horizontal = 1,
        Diagonal = 2,
        Both = Horizontal | Diagonal
    }
    public Directions directions;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        if(directions.HasFlag(Directions.Horizontal))
        {
            foreach(var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.up))
            {
                yield return pos;
            }
            foreach (var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.down))
            {
                yield return pos;
            }
            foreach (var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.left))
            {
                yield return pos;
            }
            foreach (var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.right))
            {
                yield return pos;
            }
        }
        if (directions.HasFlag(Directions.Diagonal))
        {
            foreach (var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.up + Vector2Int.right))
            {
                yield return pos;
            }
            foreach (var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.down + Vector2Int.right))
            {
                yield return pos;
            }
            foreach (var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.down + Vector2Int.left))
            {
                yield return pos;
            }
            foreach(var pos in PositionsUntilObstacle(grid, userPos, Vector2Int.up + Vector2Int.left))
            {
                yield return pos;
            }
        }
    }

    private IEnumerable<Vector2Int> PositionsUntilObstacle(BattleGrid grid, Vector2Int startPos, Vector2Int direction)
    {
        Vector2Int pos = startPos + direction;
        while(grid.IsLegalAndEmpty(pos))
        {
            yield return pos;
            pos += direction;
        }
    }

    public override int MaxDistance(BattleGrid grid)
    {
        if(directions.HasFlag(Directions.Diagonal))
            return grid.Dimensions.x + grid.Dimensions.y - 2;
        if (directions.HasFlag(Directions.Horizontal))
            return Mathf.Max(grid.Dimensions.x, grid.Dimensions.y) - 1;
        return 0;
    }
}
