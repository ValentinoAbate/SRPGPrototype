using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetPatternGeneratorStatCompare : TargetPatternGenerator
{
    public Stats.StatName stat;
    public Stats.StatName userStat = Stats.StatName.None;
    public int constant = 1;
    public ComparisonOperator comparison;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        int threshold = userStat == Stats.StatName.None ? constant : user.GetStat(userStat);
        return grid.FindAll((u) => UnitPredicate(u, threshold)).Select((u) => u.Pos);
    }

    private bool UnitPredicate(Unit u, int threshold)
    {
        int value = u.GetStat(stat);
        return comparison.Evaluate(threshold, value);
    }
}
