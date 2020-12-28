using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ComparisonOperator
{
    None = 0,
    EqualTo = 1,
    LessThan = 2,
    GreaterThan = 4,
    LessThanOrEqualTo = LessThan | EqualTo,
    GreaterThanOrEqualTo = GreaterThan | EqualTo,
}

public static class ComparisonOperatorExtensions
{
    public static bool Evaluate(this ComparisonOperator op, int threshold, int value)
    {
        if (op.HasFlag(ComparisonOperator.EqualTo) && threshold == value)
            return true;
        if (op.HasFlag(ComparisonOperator.GreaterThan) && value > threshold)
            return true;
        if (op.HasFlag(ComparisonOperator.LessThan) && value < threshold)
            return true;
        return false;
    }
}
