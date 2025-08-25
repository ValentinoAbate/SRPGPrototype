using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectModifyStat : ActionEffect, IDamagingActionEffect
{
    public Stats.StatName stat;
    public ActionNumber number;
    public override bool UsesPower => usesPower;
    [SerializeField] private bool usesPower = true;
    public bool DealsDamage => stat == Stats.StatName.HP;
    private int actionValue = 0;

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        actionValue = number.ActionValue(grid, action, user, GetTargetList(grid, targetPositions).Count);
        if (UsesPower)
            actionValue += user.Power.Value;
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        target.ModifyStat(grid, stat, CalculateValue(grid, action, user, target, targetData), user);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null;
    }

    public int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        return DealsDamage ? number.BaseValue(action, user) : 0;
    }

    private int CalculateValue(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return actionValue + number.TargetValue(grid, action, user, target, targetData);
    }

    public int CalculateDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData, bool simulation)
    {
        return DealsDamage ? CalculateValue(grid, action, user, target, targetData) : 0;
    }
}
