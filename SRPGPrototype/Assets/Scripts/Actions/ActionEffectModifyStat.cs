using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectModifyStat : ActionEffect
{
    public Stats.StatName stat;
    public ActionNumber number;
    public override bool UsesPower => usesPower;
    [SerializeField] private bool usesPower = true;
    private int actionValue = 0;

    public override void Initialize(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        actionValue = number.ActionValue(grid, action, user, targetPositions);
        if (UsesPower)
            actionValue += user.Power.Value;
    }

    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var target = grid.Get(targetData.targetPos);
        if (target == null)
            return;
        var value = actionValue + number.TargetValue(grid, action, user, target, targetData);
        target.ModifyStat(grid, stat, value, user);
    }
}
