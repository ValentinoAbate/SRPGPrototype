﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnDeathAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onDeath += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit self, Unit killedBy);
}
