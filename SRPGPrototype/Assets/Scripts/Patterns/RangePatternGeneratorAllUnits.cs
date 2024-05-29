using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RangePatternGeneratorAllUnits : RangePatternGenerator
{ 
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos)
    {
        return grid.Select((u) => u.Pos);
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.Dimensions.x + grid.Dimensions.y - 2;
    }
}
