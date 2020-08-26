using System.Collections;
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
    public override List<Vector2Int> Generate(BattleGrid grid, Unit user)
    {
        var ret = new List<Vector2Int>();
        if(directions.HasFlag(Directions.Horizontal))
        {
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.up));
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.down));
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.left));
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.right));
        }
        if (directions.HasFlag(Directions.Diagonal))
        {
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.up + Vector2Int.right));
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.down + Vector2Int.right));
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.down + Vector2Int.left));
            ret.AddRange(PositionsUntilObstacle(grid, user.Pos, Vector2Int.up + Vector2Int.left));
        }
        return ret;
    }

    private List<Vector2Int> PositionsUntilObstacle(BattleGrid grid, Vector2Int startPos, Vector2Int direction)
    {
        Vector2Int pos = startPos + direction;
        var ret = new List<Vector2Int>();
        while(grid.IsLegalAndEmpty(pos))
        {
            ret.Add(pos);
            pos += direction;
        }
        return ret;
    }
}
