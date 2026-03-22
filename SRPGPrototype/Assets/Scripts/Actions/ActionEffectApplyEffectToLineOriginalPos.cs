using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectApplyEffectToLineOriginalPos : ActionEffect, IDamagingActionEffect
{
    public override bool UsesPower => effect.UsesPower;
    public bool DealsDamage => effect.CanDealDamage;

    [SerializeField] private ActionEffect effect;

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        base.Initialize(grid, action, sub, user, targetPositions);
        effect.Initialize(grid, action, sub, user, targetPositions);
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (!user.Pos.IsOnGridLine(targetData.originalUserPos))
            return;
        var pos = user.Pos;
        var direction = pos.DirectionTo(targetData.originalUserPos);
        pos += direction;
        while(pos != targetData.originalUserPos && grid.IsLegal(pos))
        {
            var unit = grid.Get(pos);
            var unitTargetData = new PositionData(pos, targetData.selectedPos, targetData.originalUserPos);
            if (effect.IsValidTarget(grid, action, sub, user, unit, unitTargetData))
            {
                effect.ApplyEffect(grid, action, sub, user, unit, unitTargetData);
            }
            pos += direction;
        }
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return user.Pos.IsOnGridLine(targetData.originalUserPos);
    }

    public int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        return effect is IDamagingActionEffect damageEffect ? damageEffect.ActionMacroDamage(grid, action, sub, user, indices) : 0;
    }

    public int CalculateDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData, bool simulation)
    {
        return effect is IDamagingActionEffect damageEffect ? damageEffect.CalculateDamage(grid, action, sub, user, target, targetData, simulation) : 0;
    }

    public override bool CanSave(bool isBattle)
    {
        return effect.CanSave(isBattle);
    }

    public override string Save(bool isBattle)
    {
        return effect.Save(isBattle);
    }

    public override void Load(string data, bool isBattle)
    {
        effect.Load(data, isBattle);
    }
}
