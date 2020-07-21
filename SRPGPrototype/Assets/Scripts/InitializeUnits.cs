using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeUnits : MonoBehaviour
{
    public BattleGrid grid;
    public PlayerPhase playerPhase;
    public EnemyPhase enemyPhase;
    // Start is called before the first frame update
    void Start()
    {
        var units = GetComponentsInChildren<Unit>();
        foreach (var unit in units)
        {
            grid.Add(grid.GetPos(unit.transform.position), unit);
            unit.transform.position = grid.GetSpace(unit.Pos);
            if(unit is PlayerUnit)
            {
                playerPhase.units.Add(unit as PlayerUnit);
            }
            else if(unit is EnemyUnit)
            {
                enemyPhase.units.Add(unit as EnemyUnit);
            }
        }
    }
}
