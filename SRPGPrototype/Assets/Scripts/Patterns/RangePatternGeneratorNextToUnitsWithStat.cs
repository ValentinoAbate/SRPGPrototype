using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorNextToUnitsWithStat : RangePatternGeneratorNextToUnits
{
    [SerializeField] private ComparisonOperator op;
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private int threshold;

    protected override bool UnitPredicate(BattleGrid grid, Unit unit, Vector2Int userPos, Unit user)
    {
        return op.Evaluate(threshold, unit.GetStat(stat));
    }
}
