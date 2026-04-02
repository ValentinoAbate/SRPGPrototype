using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectBleedOnDamaged : ProgramEffectAddOnDamagedAbility
{
    public override string AbilityName => "Bleed when damaged";

    public override void Ability(BattleGrid grid, Unit self, Unit source, int amount)
    {
        EncounterEventManager.EnqueueDelayedEffect(() => AbilityUtils.TriggerBleed(grid));
    }
}
