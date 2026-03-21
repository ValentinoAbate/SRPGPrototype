using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnSummonAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onSummon += OnSummon;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.OnSummonFn += OnSummon;
    }

    protected abstract void OnSummon(BattleGrid grid, Unit self, Unit summoned);
}
