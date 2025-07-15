using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnDamagedAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onDamaged += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit self, Unit source, int amount);
}
