using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorAllUnitsWithTargetStat : TargetPatternGeneratorAllUnitsOnTeams
{
    [SerializeField] private Stats.StatName stat = Stats.StatName.None;
    [SerializeField] private ComparisonOperator comparison;

    protected override bool IsTargetValid(BattleGrid grid, Unit user, Unit target, Vector2Int targetPos)
    {
        // Threshold can be a user stat or a constant
        if (stat == Stats.StatName.None || !grid.TryGet(targetPos, out var originalTarget))
            return false;
        return comparison.Evaluate(originalTarget.GetStat(stat), target.GetStat(stat));
    }
}
