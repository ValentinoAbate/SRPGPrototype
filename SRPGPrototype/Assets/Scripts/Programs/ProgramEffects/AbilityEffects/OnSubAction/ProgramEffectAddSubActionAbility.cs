using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProgramEffectAddSubActionAbility : ProgramEffectAddAbility
{
    public enum Timing
    {
        AfterSubAction,
        BeforeSubAction
    }
    [SerializeField] private Timing timing;
    [SerializeField] private SubAction.Type[] subTypes = new SubAction.Type[0];
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

    public virtual void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions, SubAction.Type overrideSubType = SubAction.Type.None)
    {
        if (!HasAppicableSubType(subAction, overrideSubType))
        {
            return;
        }
        OnSubAction(grid, action, subAction, user, targets, targetPositions);
    }

    private bool HasAppicableSubType(SubAction subAction, SubAction.Type overrideSubType)
    {
        if (subTypes.Length <= 0)
            return true;
        if (overrideSubType != SubAction.Type.None)
            return subTypes.Contains(overrideSubType);
        return subAction.HasAnySubType(subTypes);
    }
    public abstract void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions);
}
