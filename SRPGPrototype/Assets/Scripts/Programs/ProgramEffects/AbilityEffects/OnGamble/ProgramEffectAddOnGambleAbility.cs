using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnGambleAbility : ProgramEffectAddAbility
{
    public override string AbilityName => throw new System.NotImplementedException();

    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onGamble += OnGamble;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.OnGambleFn += OnGamble;
    }

    protected abstract void OnGamble(BattleGrid grid, Action action, Unit unit, bool succeeded);
}
