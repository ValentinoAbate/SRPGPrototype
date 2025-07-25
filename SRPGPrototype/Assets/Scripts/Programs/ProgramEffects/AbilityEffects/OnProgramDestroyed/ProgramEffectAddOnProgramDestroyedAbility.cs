using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnProgramDestroyedAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onProgramDestroyed += Ability;
    }

    protected abstract void Ability(Program p, BattleGrid grid, Unit user);
}
