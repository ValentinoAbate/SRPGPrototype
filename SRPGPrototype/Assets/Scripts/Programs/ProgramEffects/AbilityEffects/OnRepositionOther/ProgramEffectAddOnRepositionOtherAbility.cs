using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnRepositionOtherAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onRepositionOther += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit source, Unit target);
}
