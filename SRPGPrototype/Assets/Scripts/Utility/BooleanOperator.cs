using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BooleanOperator
{
    AND,
    OR,
}

public static class BooleanOperatorExtensions
{
    public static bool Evaluate(this BooleanOperator op, Enum flagsEnum, Enum comparisonValues)
    {
        if (op == BooleanOperator.AND)
        {
            return flagsEnum.HasFlag(comparisonValues);
        }
        else if(op == BooleanOperator.OR)
        {
            try
            {
                return (Convert.ToInt32(flagsEnum) | Convert.ToInt32(comparisonValues)) > 0;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }
}
