using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorAllEmptyTiles : RangePatternGenerator
{ 
    public override List<Vector2Int> Generate(BattleGrid grid, Unit user)
    {
        return grid.EmptyPositions;
    }
}
