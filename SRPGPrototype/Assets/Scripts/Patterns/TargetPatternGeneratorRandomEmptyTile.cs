using Extensions.VectorIntDimensionUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetPatternGeneratorRandomEmptyTile : TargetPatternGenerator
{ 
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        if (!grid.EmptyPositions.Any())
            yield break;
        yield return RandomUtils.RandomU.instance.Choice(grid.EmptyPositions);
    }
}
