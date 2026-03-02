using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionUtils
{
    // Return an estimation of the the damage that would be dealt if the user hit the target with the given sub action/action
    // Only takes into account Action
    public static int SimulateDamageCalc(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, ActionEffect.PositionData targetData, List<Vector2Int> targetPositions)
    {
        int damageTotal = 0;
        foreach(var effect in sub.Effects)
        {
            if (effect.AffectUser || !(effect is IDamagingActionEffect damageEffect))
                continue;
            effect.Initialize(grid, action, sub, user, targetPositions);
            damageTotal += damageEffect.CalculateDamage(grid, action, sub, user, target, targetData, true);
        }
        damageTotal += target.Break;
        return damageTotal;
    }

    public static int Uses(Action a) => a.TimesUsed;
    public static int UsesTurn(Action a) => a.TimesUsedThisTurn;
    public static int UsesEncounter(Action a) => a.TimesUsedThisBattle;
}
