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

    public int BaseValue(Action action, Unit user)
    {
        if (type == Type.NumberOfTargets || type == Type.TargetStat)
            return 0;
        return BasicValue(action, user);
    }

    public int ActionValue(BattleGrid grid, Action action, Unit user, int numTargets)
    {
        if (type == Type.TargetStat)
            return 0;
        return type switch
        {
            Type.NumberOfTargets => Value(numTargets),
            _ => BasicValue(action, user)
        };
    }

    private int BasicValue(Action action, Unit user)
    {
        if (type == Type.Constant)
            return constant;
        return Value(type switch
        {
            Type.APCost => user == null ? action.APCost : action.APCost - user.Speed.Value,
            Type.TimeUsed => action.TimesUsed,
            Type.TimesUsedThisBattle => action.TimesUsedThisBattle,
            Type.TimesUsedThisTurn => action.TimesUsedThisTurn,
            Type.UserStat => unitNumber.Value(user),
            _ => 0
        });
    }

    public int TargetValue(BattleGrid grid, Action action, Unit user, Unit target, ActionEffect.PositionData targetData)
    {
        if(type != Type.TargetStat)
            return 0;
        // Type is targetStat
        return Value(unitNumber.Value(target));

    }

    private int Value(int number)
    {
        int modNumber = baseAmount + ((number + modifier) * multiplier);
        return Mathf.Clamp(modNumber, min, max);
    }
}
