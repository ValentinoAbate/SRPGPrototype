using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffect : MonoBehaviour
{
    public virtual bool UsesPower => false;
    public bool CanDealDamage
    {
        get
        {
            return this is IDamagingActionEffect damageEffect && damageEffect.DealsDamage;
        }
    }

    public SubAction.Type StandaloneSubActionType => standaloneSubActionType;
    [SerializeField] SubAction.Type standaloneSubActionType;
    public SubAction.Options SubActionOptions => standaloneSubActionOptions;
    [SerializeField] SubAction.Options standaloneSubActionOptions;
    public bool AffectUser => affectUser;
    [SerializeField] private bool affectUser = false;
    public bool IgnoreInValidRangeCalcs => ignoreInValidRangeCalcs;
    [SerializeField] private bool ignoreInValidRangeCalcs = false;

    public virtual void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions) { }
    public abstract void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData);
    public bool IsValidTarget(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return !ignoreInValidRangeCalcs && IsValidTargetInternal(grid, action, sub, user, target, targetData);
    }
    protected abstract bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData);

    public struct PositionData
    {
        // The position of the current target tile
        public Vector2Int targetPos;
        // The position that was originally selected as the target for the action
        public Vector2Int selectedPos;

        public PositionData(Vector2Int target, Vector2Int selected)
        {
            targetPos = target;
            selectedPos = selected;
        }
    }

    protected List<Unit> GetTargetList(BattleGrid grid, IReadOnlyList<Vector2Int> targetPositions)
    {
        var targets = new List<Unit>(targetPositions.Count);
        foreach(var pos in targetPositions)
        {
            if(grid.TryGet(pos, out var unit))
            {
                targets.Add(unit);
            }
        }
        return targets;
    }

}
