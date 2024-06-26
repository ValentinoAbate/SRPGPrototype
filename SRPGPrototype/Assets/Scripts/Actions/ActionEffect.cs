﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public virtual void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions) { }
    public abstract void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData);

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
        return targetPositions.Select((pos) => grid.Get(pos)).Where((c) => c != null).ToList();
    }

}
