using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectUseActionOnKillWithStat : ProgramEffectUseActionOnKill
{
    [SerializeField] private ComparisonOperator op;
    [SerializeField] private int threshold;
    [SerializeField] private Stats.StatName stat;
    protected override bool UnitPredicate(BattleGrid grid, Unit killed, Unit self)
    {
        return base.UnitPredicate(grid, killed, self) && op.Evaluate(threshold, killed.GetStat(stat));
    }
}
