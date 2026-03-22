using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectBleedOnDeath : ProgramEffectAddOnDeathAbility
{
    public override string AbilityName => "Bleed when destroyed";

    public override void Ability(BattleGrid grid, Unit self, Unit killedBy)
    {
        AbilityUtils.TriggerBleed(grid);
    }
}
