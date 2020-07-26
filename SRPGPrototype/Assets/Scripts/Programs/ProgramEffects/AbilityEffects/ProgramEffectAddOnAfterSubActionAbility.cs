using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnAfterSubActionAbility : ProgramEffect
{
    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.abilityOnAfterSubAction += Ability;
    }

    public abstract void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Vector2Int> targetPositions);
}
