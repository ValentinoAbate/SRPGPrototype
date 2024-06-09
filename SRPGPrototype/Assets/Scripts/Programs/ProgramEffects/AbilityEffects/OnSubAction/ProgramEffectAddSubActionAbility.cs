using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddSubActionAbility : ProgramEffectAddAbility
{
    public enum Timing
    {
        AfterSubAction,
        BeforeSubAction
    }
    [SerializeField] private Timing timing;
    protected override void AddAbility(ref Shell.CompileData data)
    {
        if (timing == Timing.BeforeSubAction)
        {
            data.onBeforeSubAction += Ability;
        }
        else
        {
            data.onAfterSubAction += Ability;
        }
    }

    public abstract void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions);
}
