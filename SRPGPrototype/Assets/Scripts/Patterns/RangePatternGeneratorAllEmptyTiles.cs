using Extensions.VectorIntDimensionUtils;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorAllEmptyTiles : RangePatternGenerator
{ 
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        return grid.EmptyPositions;
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        if (!grid.IsEmpty(targetPos))
            yield break;
        foreach (var pos in grid.Dimensions.Enumerate())
        {
            yield return pos;
        }
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.MaxGridDistance;
    }
}
