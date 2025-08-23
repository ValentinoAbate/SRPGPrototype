using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddRepairAbility : ProgramEffectAddAbility
{
    public Stats.RepairAbilities abilities = Stats.RepairAbilities.None;
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.stats.RepairAbilityFlags |= abilities;
    }
}
