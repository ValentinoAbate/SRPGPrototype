using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ActionNumber
{
    public enum Type
    {
        Constant,
        APCost,
        NumberOfTargets,
        TimeUsed,
        TimesUsedThisBattle,
        TimesUsedThisTurn,
        UserStat,
        TargetStat,
    }

    [SerializeField]
    private int min = int.MinValue;
    [SerializeField]
    private int max = int.MaxValue;
    [SerializeField]
    private int constant = 1;
    [SerializeField]
    private int baseAmount = 0;
    [SerializeField]
    private int modifier = 0;
    [SerializeField]
    private int multiplier = 1;
    [SerializeField]
    UnitNumber unitNumber = new UnitNumber();
    [SerializeField]
    private Type type = Type.Constant;

    public int ActionValue(BattleGrid grid, Action action, Unit user, int numTargets)
    {
        if(type == Type.Constant)
            return constant;
        if (type == Type.TargetStat)
            return 0;
        int number = 0;
        if(type == Type.APCost)
        {
            number = action.APCost - user.Speed.Value;
        }
        else if(type == Type.NumberOfTargets)
        {
            number = numTargets;
        }
        else if(type == Type.TimeUsed)
        {
            number = action.TimesUsed;
        }
        else if(type == Type.TimesUsedThisBattle)
        {
            number = action.TimesUsedThisBattle;
        }
        else if (type == Type.TimesUsedThisTurn)
        {
            number = action.TimesUsedThisTurn;
        }
        else if (type == Type.UserStat)
        {
            number = unitNumber.Value(user);
        }
        int modNumber = baseAmount + ((number + modifier) * multiplier);
        return Mathf.Clamp(modNumber, min, max);
    }

    public int TargetValue(BattleGrid grid, Action action, Unit user, Unit target, ActionEffect.PositionData targetData)
    {
        if(type != Type.TargetStat)
            return 0;
        // Type is targetStat
        int number = unitNumber.Value(target);
        int modNumber = baseAmount + ((number + modifier) * multiplier);
        return Mathf.Clamp(modNumber, min, max);
    }
}
