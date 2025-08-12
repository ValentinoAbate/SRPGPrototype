using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectSpawnUnit : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (!IsValidTargetInternal(grid, action, sub, user, target, targetData))
            return;
        SpawnUnit(grid, targetData.targetPos);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return grid.IsLegal(targetData.targetPos) && (grid.IsEmpty(targetData.targetPos) || grid.Get(targetData.targetPos).Dead);
    }

    protected abstract GameObject GetUnitPrefab();

    private void SpawnUnit(BattleGrid grid, Vector2Int pos)
    {
        var previousUnit = grid.Get(pos);
        if(previousUnit != null)
        {
            if (previousUnit.Dead)
            {
                grid.Remove(previousUnit);
            }
            else
            {
                return;
            }
        }
        // Spawn Unit
        var unit = Instantiate(GetUnitPrefab()).GetComponent<Unit>();
        // Add Unit to the grid
        grid.Add(pos, unit);
        unit.transform.position = grid.GetSpace(unit.Pos);
        if (unit.UnitTeam is Unit.Team.Player)
        {
            int maxHotkeyIndex = -1;
            foreach (var u in grid)
            {
                if(u.HotkeyIndex > maxHotkeyIndex)
                {
                    maxHotkeyIndex = u.HotkeyIndex;
                }
            }
            if(maxHotkeyIndex < BattleUI.MaxHotkeyIndex)
            {
                unit.HotkeyIndex = maxHotkeyIndex + 1;
            }
        }
    }
}
