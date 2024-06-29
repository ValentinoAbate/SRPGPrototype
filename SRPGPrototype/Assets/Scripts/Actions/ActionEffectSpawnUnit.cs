using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectSpawnUnit : ActionEffect
{
    public const int spawnDamage = 2;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
        {
            SpawnUnit(grid, targetData.targetPos);
        }
        else if (target.Dead)
        {
            grid.Remove(target);
            SpawnUnit(grid, targetData.targetPos);
        }
        else
        {
            target.Damage(grid, spawnDamage, user);
            if (target.Dead)
            {
                grid.Remove(target);
                SpawnUnit(grid, targetData.targetPos);
            }
        }
    }

    protected abstract GameObject GetUnitPrefab();

    private void SpawnUnit(BattleGrid grid, Vector2Int pos)
    {
        // Spawn Unit
        var unit = Instantiate(GetUnitPrefab()).GetComponent<Unit>();
        // Add Unit to the grid
        grid.Add(pos, unit);
        unit.transform.position = grid.GetSpace(unit.Pos);
    }
}
