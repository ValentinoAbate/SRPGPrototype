using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : Grid.Grid<Unit>
{
    [SerializeField] private Vector2Int dimensions = new Vector2Int(8, 8);
    public override Vector2Int Dimensions => dimensions;

    public bool MainPlayerDead
    {
        get
        {
            foreach (var unit in this)
            {
                if (unit is PlayerUnit playerUnit && playerUnit.IsMain)
                {
                    return playerUnit.Dead;
                }
            }
            return true;
        }
    }

    public bool TryGetPlayer(int unitIndex, out PlayerUnit player)
    {
        foreach (var unit in this)
        {
            if (unit is PlayerUnit playerUnit && playerUnit.UnitIndex == unitIndex)
            {
                player = playerUnit;
                return true;
            }
        }
        player = null;
        return false;
    }

    public IEnumerable<PlayerUnit> PlayerUnits
    {
        get
        {
            foreach (var unit in this)
            {
                if (unit is PlayerUnit playerUnit)
                    yield return playerUnit;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    public void SetDimensions(int x, int y)
    {
        dimensions = new Vector2Int(x, y);
        Initialize();
    }

    public override bool Add(Vector2Int pos, Unit obj)
    {
        if(base.Add(pos, obj))
        {
            return true;
        }
        return false;
    }

    public bool CanLinkOut(out Unit.Interference interferenceLevel, out int numInterferers)
    {
        // Link Out Button
        interferenceLevel = Unit.Interference.None;
        numInterferers = 0;
        foreach (var unit in this)
        {
            if (unit.InterferenceLevel == Unit.Interference.Jamming)
            {
                if (interferenceLevel != Unit.Interference.Jamming)
                {
                    numInterferers = 1;
                    interferenceLevel = Unit.Interference.Jamming;
                }
                else
                {
                    ++numInterferers;
                }
                continue;
            }

            if (interferenceLevel == Unit.Interference.Jamming)
                continue;
            if (unit.InterferenceLevel == Unit.Interference.Low)
            {
                if (interferenceLevel != Unit.Interference.Low)
                {
                    numInterferers = 1;
                    interferenceLevel = Unit.Interference.Low;
                }
                else
                {
                    ++numInterferers;
                }
            }
        }
        return interferenceLevel == Unit.Interference.None || (interferenceLevel == Unit.Interference.Low && numInterferers <= 2);
    }

    public bool IsAdjacent(Unit unit1, Unit unit2)
    {
        int xDist = System.Math.Abs(unit1.Pos.y - unit2.Pos.y);
        int yDist = System.Math.Abs(unit1.Pos.x - unit2.Pos.x);
        return (xDist + yDist) == 1;
    }
}
