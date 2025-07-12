using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProgramFilters
{
    public static bool GivesMoveAction(Program p)
    {
        return GivesActionOfType(p, Action.Type.Move);
    }

    public static bool GivesWeaponAction(Program p)
    {
        return GivesActionOfType(p, Action.Type.Weapon);
    }

    public static bool GivesHybridAction(Program p)
    {
        return GivesActionOfType(p, Action.Type.Hybrid);
    }

    public static bool GivesRepositionSkill(Program p)
    {
        return GivesActionOfTypeAndSubType(p, Action.Type.Skill, SubAction.Type.Reposition);
    }

    public static bool GivesActionOfType(Program p, Action.Type type)
    {
        foreach (var effect in p.Effects)
        {
            if (effect is ProgramEffectAddAction addActionEffect && addActionEffect.action.ActionType == type)
                return true;
        }
        return false;
    }

    public static bool GivesActionOfSubType(Program p, SubAction.Type subType)
    {
        foreach (var effect in p.Effects)
        {
            if (effect is ProgramEffectAddAction addActionEffect)
            {
                foreach(var subAction in addActionEffect.action.SubActions)
                {
                    if (subAction.HasSubType(subType))
                        return true;
                }
            }
        }
        return false;
    }

    public static bool GivesActionOfTypeAndSubType(Program p, Action.Type type, SubAction.Type subType)
    {
        foreach (var effect in p.Effects)
        {
            if (effect is ProgramEffectAddAction addActionEffect && addActionEffect.action.ActionType == type)
            {
                foreach (var subAction in addActionEffect.action.SubActions)
                {
                    if (subAction.HasSubType(subType))
                        return true;
                }
            }
        }
        return false;
    }

    public static bool HasModifierOrAbility(Program p) => HasModifierEffect(p) || HasAbility(p);

    public static bool HasModifierEffect(Program p) => p.ModifierEffects.Length > 0;

    public static bool HasAbility(Program p) => HasEffect<ProgramEffectAddAbility>(p);

    public static bool GivesCapacity(Program p) => HasEffect<ProgramEffectModifyCapacity>(p);

    public static bool HasEffect<T>(Program p) where T : ProgramEffect
    {
        foreach (var effect in p.Effects)
        {
            if (effect is T)
                return true;
        }
        return false;
    }

    public static bool IsWhite(Program p) => IsColor(p, Program.Color.White);

    public static bool IsRed(Program p) => IsColor(p, Program.Color.Red);

    public static bool IsBlue(Program p) => IsColor(p, Program.Color.Blue);

    public static bool IsColor(Program p, Program.Color color) => p.color == color && p.GetComponent<ProgramVariantColor>() == null;

    public static bool HasAttributes(Program p, Program.Attributes attributes) => p.attributes.HasFlag(attributes);

    public static bool IsGamble(Program p) => HasAttributes(p, Program.Attributes.Gamble);
}
