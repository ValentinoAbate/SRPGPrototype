using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetPatternGeneratorStatCompare : TargetPatternGenerator
{
    [System.Flags]
    public enum Comparisons
    { 
        None = 0,
        EqualTo = 1,
        LessThan = 2,
        GreaterThan = 4,
        GreaterThanOrEqualTo = GreaterThan | EqualTo,
        LessThanOrEqualTo = LessThan | EqualTo,
    }

    public Stats.StatName stat;
    public Stats.StatName userStat = Stats.StatName.None;
    public int constant = 1;
    public Comparisons comparisons;

    public override List<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        return grid.FindAll((u) => UnitPredicate(u, user)).Select((u) => u.Pos).ToList();
    }

    private bool UnitPredicate(Unit u, Unit user)
    {
        int value = u.GetStat(stat);
        int threshold = userStat == Stats.StatName.None ? constant : user.GetStat(userStat);
        if (comparisons.HasFlag(Comparisons.EqualTo) && value == threshold)
            return true;
        if (comparisons.HasFlag(Comparisons.LessThan) && value < threshold)
            return true;
        if (comparisons.HasFlag(Comparisons.GreaterThan) && value > threshold)
            return true;
        return false;
    }
}
