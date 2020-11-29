using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerNumberOfTargets : ProgramTriggerCondition
{
    public override bool Completed => progress >= times;

    [SerializeField] private int times = 5;
    [SerializeField] private ComparisonOperator comparison = ComparisonOperator.GreaterThanOrEqualTo;
    [SerializeField] private int threshold = 3;

    private int progress = 0;

    private readonly List<Action> actions = new List<Action>();

    public override string RevealedConditionText
    {
        get
        {
            string ret = "hit ";
            if (comparison.HasFlag(ComparisonOperator.GreaterThan))
            {
                ret += " more than " + (comparison.HasFlag(ComparisonOperator.EqualTo) ? (threshold - 1) : threshold);
            }
            else if (comparison.HasFlag(ComparisonOperator.LessThan))
            {
                ret += " less than " + (comparison.HasFlag(ComparisonOperator.EqualTo) ? (threshold + 1) : threshold);
            }
            else
            {
                ret += threshold;
            }
            ret += " targets " + times + (times > 1 ? " times" : " time") + "(" + progress + "/" + times + ")";
            return ret;
        }
    }

    public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        actions.Clear();
        // Log actions from the program
        actions.AddRange(program.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => (e as ProgramEffectAddAction).action));
        // Add the check
        data.onAfterSubAction += Check;
    }
    private void Check(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        if (!actions.Contains(action))
            return;
        if(comparison.Evaluate(threshold, targets.Count))
        {
            ++progress;
        }
    }
}
