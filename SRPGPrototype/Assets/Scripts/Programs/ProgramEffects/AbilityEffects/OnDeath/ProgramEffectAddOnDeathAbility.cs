using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnDeathAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(Program program, ref Shell.CompileData data)
    {
        data.abilityOnDeath += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit self, Unit killedBy);
}
