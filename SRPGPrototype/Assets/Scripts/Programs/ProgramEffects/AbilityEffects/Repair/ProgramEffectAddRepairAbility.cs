using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddRepairAbility : ProgramEffectAddAbility
{
    public Stats.RepairAbilities abilities = Stats.RepairAbilities.None;
    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.stats.RepairAbilityFlags |= abilities;
    }
}
