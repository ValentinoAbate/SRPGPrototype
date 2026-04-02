using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorNextToUnitsWithTags : RangePatternGeneratorNextToUnits
{
    [SerializeField] private Unit.Tags tags;
    [SerializeField] private BooleanOperator op;
    protected override bool UnitPredicate(BattleGrid grid, Unit unit, Vector2Int userPos, Unit user)
    {
        return op.Evaluate(unit.UnitTags, tags);
    }
}
