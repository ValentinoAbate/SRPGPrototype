using Extensions.VectorIntDimensionUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorAllUnitsWithTags : RangePatternGenerator
{
    [SerializeField] private Unit.Tags tags;
    [SerializeField] private BooleanOperator op;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        foreach(var unit in grid.FindUnitsWithTags(tags, op))
        {
            yield return unit.Pos;
        }
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        if (grid.IsEmpty(targetPos))
        {
            yield break;
        }
        foreach (var pos in grid.Dimensions.Enumerate())
        {
            yield return pos;
        }
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.Dimensions.x + grid.Dimensions.y - 2;
    }
}
