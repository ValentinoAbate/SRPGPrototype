using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorStatCompare : TargetPatternGenerator
{
    public Stats.StatName stat;
    public Stats.StatName userStat = Stats.StatName.None;
    public int constant = 1;
    public ComparisonOperator comparison;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        // Threshold can be a user stat or a constant
        int threshold = userStat == Stats.StatName.None ? constant : user.GetStat(userStat);
        foreach(var unit in grid)
        {
            if(comparison.Evaluate(threshold, unit.GetStat(stat)))
            {
                yield return unit.Pos;
            }
        }
    }
}
