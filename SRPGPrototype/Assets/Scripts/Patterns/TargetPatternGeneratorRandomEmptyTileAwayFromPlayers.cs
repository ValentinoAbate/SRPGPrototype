using Extensions.VectorIntDimensionUtils;
using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetPatternGeneratorRandomEmptyTileAwayFromPlayers : TargetPatternGenerator
{ 
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        if (!grid.EmptyPositions.Any())
            yield break;
        var set = new WeightedSet<Vector2Int>();
        var players = grid.FindAll(UnitFilters.IsPlayer);
        foreach(var pos in grid.EmptyPositions)
        {
            float weight = 0;
            foreach(var unit in players)
            {
                weight += Mathf.Pow(Vector2IntUtils.ManhattanDistance(pos, unit.Pos), 2);
            }
            set.Add(pos, weight);
        }
        yield return RandomUtils.RandomU.instance.Choice(grid.EmptyPositions);
    }
}
