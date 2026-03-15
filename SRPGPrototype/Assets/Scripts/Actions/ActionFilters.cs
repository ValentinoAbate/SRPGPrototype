using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionFilters
{
    public static bool IsType(Action action, Action.Type type) => action.ActionType == type;
    public static bool IsAnyType(Action action, IEnumerable<Action.Type> types)
    {
        foreach(var type in types)
        {
            if (IsType(action, type))
                return true;
        }
        return false;
    }
    public static bool IsAnyTypeOptional(Action action, ICollection<Action.Type> types)
    {
        if (types.Count <= 0)
            return true;
        return IsAnyType(action, types);
    }

    public static bool IsAnyTypeAndSubTypeOptional(Action action, ICollection<Action.Type> types, SubAction sub, ICollection<SubAction.Type> subTypes)
    {
        return IsAnyTypeOptional(action, types) && sub.HasAnySubTypeOptional(subTypes);
    }
}
