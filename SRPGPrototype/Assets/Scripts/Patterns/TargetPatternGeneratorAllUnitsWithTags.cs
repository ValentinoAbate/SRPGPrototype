using Extensions.VectorIntDimensionUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorAllUnitsWithTags : TargetPatternGenerator
{
    [SerializeField] private Unit.Tags tags;
    [SerializeField] private BooleanOperator op;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        foreach (var unit in grid.FindUnitsWithTags(tags, op))
        {
            yield return unit.Pos;
        }
    }
}
