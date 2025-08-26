using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorStartingTile : RangePatternGenerator
{
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        yield return user.StartingPos;
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        return grid.AllPositions;
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.MaxGridDistance;
    }
}
