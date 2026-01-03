using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorAllUnitsWithStat : TargetPatternGenerator
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private Stats.StatName userStat = Stats.StatName.None;
    [SerializeField] private int constant = 1;
    [SerializeField] private ComparisonOperator comparison;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        // Threshold can be a user stat or a constant
        int threshold = userStat == Stats.StatName.None ? constant : user.GetStat(userStat);
        foreach(var unit in grid)
        {
            // Check if the target's stat meets the threshold
            if(comparison.Evaluate(threshold, unit.GetStat(stat)))
            {
                yield return unit.Pos;
            }
        }
    }
}
