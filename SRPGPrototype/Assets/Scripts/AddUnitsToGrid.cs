using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddUnitsToGrid : MonoBehaviour
{
    public BattleGrid grid;
    // Start is called before the first frame update
    void Start()
    {
        var units = GetComponentsInChildren<Combatant>();
        foreach (var unit in units)
        {
            grid.Add(grid.GetPos(unit.transform.position), unit);
            unit.transform.position = grid.GetSpace(unit.Pos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
