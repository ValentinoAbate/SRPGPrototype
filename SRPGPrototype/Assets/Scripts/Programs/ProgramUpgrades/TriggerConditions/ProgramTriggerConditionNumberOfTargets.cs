using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerConditionNumberOfTargets : ProgramTriggerConditionResetTrigger
{
    [SerializeField] private ComparisonOperator comparison = ComparisonOperator.GreaterThanOrEqualTo;
    [SerializeField] private int threshold = 3;

    protected override string ProgressConditionText
    {
        get
        {
            string ret = "Hit ";
            if (comparison.HasFlag(ComparisonOperator.GreaterThan))
            {
                ret += (comparison.HasFlag(ComparisonOperator.EqualTo) ? threshold : threshold + 1) + " or more ";
            }
            else if (comparison.HasFlag(ComparisonOperator.LessThan))
            {
                ret += (comparison.HasFlag(ComparisonOperator.EqualTo) ? threshold : threshold - 1) + " or fewer ";
            }
            else
            {
                ret += threshold + " ";
            }
            return ret + "targets";
        }
    }

    protected override int ProgressChange(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        return comparison.Evaluate(threshold, targets.Count) ? 1 : 0;
    }
}
