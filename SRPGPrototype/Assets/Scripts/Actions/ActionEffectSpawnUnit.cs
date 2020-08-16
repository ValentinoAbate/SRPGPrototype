using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSpawnUnit : ActionEffect
{
    public const int spawnDamage = 2;
    public Unit unitPrefab;
    public override void ApplyEffect(BattleGrid grid, Action action, Unit user, PositionData targetData)
    {
        var unitInSpace = grid.Get(targetData.targetPos);
        if (unitInSpace == null)
        {
            SpawnUnit(grid, targetData.targetPos);
        }
        else if (unitInSpace.Dead)
        {
            grid.Remove(unitInSpace);
            SpawnUnit(grid, targetData.targetPos);
        }
        else
        {
            unitInSpace.Damage(grid, spawnDamage, user);
            if (unitInSpace.Dead)
            {
                grid.Remove(unitInSpace);
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
