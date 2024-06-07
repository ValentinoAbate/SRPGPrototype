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
        var value = actionValue + number.TargetValue(grid, action, user, target, targetData);
        target.ModifyStat(grid, stat, value, user);
    }

    public int BaseDamage(Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        return DealsDamage ? number.BaseValue(action, user) : 0;
    }
}
