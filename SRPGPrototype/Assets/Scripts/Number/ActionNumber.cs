using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionNumber : DynamicNumber
{
    public enum Type
    {
        Constant,
        APCost,
        NumberOfTargets,
        TimesUsed,
        TimesUsedThisBattle,
        TimesUsedThisTurn,
        UserStat,
        TargetStat,
    }

    [SerializeField]
    private int constant = 1;
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
            Type.APCost => GetAPCost(action, user),
            Type.TimesUsed => action == null ? 0 : action.TimesUsed,
            Type.TimesUsedThisBattle => action == null ? 0 : action.TimesUsedThisBattle,
            Type.TimesUsedThisTurn => action == null ? 0 : action.TimesUsedThisTurn,
            Type.UserStat => unitNumber.Value(user),
            _ => 0
        });
    }

    private int GetAPCost(Action action, Unit user)
    {
        if (action == null)
            return 0;
        if (user == null)
            return action.APCost;
        return action.APCost - user.Speed.Value;
    }

    public int TargetValue(BattleGrid grid, Action action, Unit user, Unit target, ActionEffect.PositionData targetData)
    {
        if(type != Type.TargetStat)
            return 0;
        // Type is targetStat
        return Value(unitNumber.Value(target));

    }


}
