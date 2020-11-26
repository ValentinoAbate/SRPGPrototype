using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorAllEmptyTiles : RangePatternGenerator
{ 
    public override List<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos)
    {
        return grid.EmptyPositions;
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.Dimensions.x + grid.Dimensions.y - 2;
    }
}
