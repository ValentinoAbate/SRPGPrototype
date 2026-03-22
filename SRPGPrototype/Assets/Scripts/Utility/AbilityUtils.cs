using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityUtils
{
    public static void TriggerBleed(BattleGrid grid)
    {
        foreach (var unit in grid)
        {
            var onDamaged = unit.GetComponent<UnitBehaviorUseActionOnUnitDamaged>();
            if (onDamaged == null || !onDamaged.TriggersOnBleed)
            {
                continue;
            }
            onDamaged.Trigger(grid);
        }
    }
}
