using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorAllUnitsWithStat : TargetPatternGeneratorAllUnitsOnTeams
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private Stats.StatName userStat = Stats.StatName.None;
    [SerializeField] private int constant = 1;
    [SerializeField] private ComparisonOperator comparison;

    protected override bool IsTargetValid(BattleGrid grid, Unit user, Unit target, Vector2Int targetPos)
    {
        // Threshold can be a user stat or a constant
        int threshold = userStat == Stats.StatName.None ? constant : user.GetStat(userStat);
        return comparison.Evaluate(threshold, target.GetStat(stat));
    }
}
