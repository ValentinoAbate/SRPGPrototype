using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnAfterSubActionAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(Program program, ref Shell.CompileData data)
    {
        data.abilityOnAfterSubAction += Ability;
    }

    public abstract void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions);
}
