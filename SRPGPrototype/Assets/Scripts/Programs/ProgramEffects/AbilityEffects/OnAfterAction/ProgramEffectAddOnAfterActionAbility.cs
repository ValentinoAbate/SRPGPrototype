using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnAfterActionAbility : ProgramEffectAddAbility
{
    [SerializeField] private string abilityName;
    protected override string AbilityName => abilityName;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onAfterAction += Ability;
    }

    protected abstract void Ability(BattleGrid grid, Action action, Unit user);
}
