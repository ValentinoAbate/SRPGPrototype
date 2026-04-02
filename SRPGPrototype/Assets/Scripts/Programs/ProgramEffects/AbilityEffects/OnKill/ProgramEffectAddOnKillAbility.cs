using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnKillAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(Shell.CompileData data)
    {
        data.onKill += Ability;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.OnKill += Ability;
    }

    protected abstract void Ability(BattleGrid grid, Unit killed, Unit self);
}
