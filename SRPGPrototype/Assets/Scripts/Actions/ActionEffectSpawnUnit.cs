using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSpawnUnit : ActionEffect
{
    public const int spawnDamage = 2;
    public Unit unitPrefab;
    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
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

    private void SpawnUnit(BattleGrid grid, Vector2Int pos)
    {
        // Spawn Unit
        var unit = Instantiate(unitPrefab).GetComponent<Unit>();
        // Add Unit to the grid
        grid.Add(pos, unit);
        unit.transform.position = grid.GetSpace(unit.Pos);
    }
}
