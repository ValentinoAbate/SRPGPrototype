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

    public static bool IsTypeAndHasSubType(Action action, bool filterByType, IEnumerable<Action.Type> types, bool filterBySubtype, IEnumerable<SubAction.Type> subTypes)
    {
        if (filterByType && !IsAnyType(action, types))
            return false;
        if (!filterBySubtype)
            return true;
        foreach (var sub in action.SubActions)
        {
            if (sub.HasAnySubType(subTypes))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsTypeAndHasSubTypeCustom(Action action, bool filterByType, IEnumerable<Action.Type> types, bool filterBySubtype, IEnumerable<SubAction.Type> subTypes, System.Predicate<SubAction> filter)
    {
        if (filterByType && !IsAnyType(action, types))
            return false;
        foreach (var sub in action.SubActions)
        {
            if (filter(sub) && (!filterBySubtype || sub.HasAnySubType(subTypes)))
            {
                return true;
            }
        }
        return false;
    }
}
