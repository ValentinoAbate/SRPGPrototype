using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectBleedOnDamaged : ProgramEffectAddOnDamagedAbility
{
    public override string AbilityName => "Bleed when Damaged";

    public override void Ability(BattleGrid grid, Unit self, Unit source, int amount)
    {
        foreach(var unit in grid)
        {
            var onDamaged = unit.GetComponent<UnitBehaviorUseActionOnUnitDamaged>();
            if(onDamaged == null || !onDamaged.TriggersOnBleed)
            {
                continue;
            }
            onDamaged.Trigger(grid);
        }
    }
}
