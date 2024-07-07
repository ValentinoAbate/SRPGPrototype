using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : Grid.Grid<Unit>
{
    [SerializeField] private Vector2Int dimensions = new Vector2Int(8, 8);
    public override Vector2Int Dimensions => dimensions;

    public PlayerUnit MainPlayer
    {
        get
        {
            foreach (var unit in this)
            {
                if (unit is PlayerUnit playerUnit && playerUnit.IsMain)
                {
                    return playerUnit;
                }
            }
            return null;
        }
    }

    public bool MainPlayerDead
    {
        get
        {
            var player = MainPlayer;
            return player == null || player.Dead;
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

    public bool CanLinkOut(out int numJammers, out int numInterferers, out int threshold)
    {
        // Link Out Button
        numJammers = 0;
        numInterferers = 0;
        foreach (var unit in this)
        {
            if(unit.InterferenceLevel == Unit.Interference.Low)
            {
                ++numInterferers;
            }
            else if(unit.InterferenceLevel == Unit.Interference.Jamming)
            {
                ++numJammers;
            }
        }
        var mainPlayer = MainPlayer;
        threshold = mainPlayer != null ? MainPlayer.LinkOutThreshold : PersistantData.main.inventory.EquippedShell.LinkOutThreshold;
        return numJammers <= 0 && numInterferers <= threshold;
    }

    public bool IsAdjacent(Unit unit1, Unit unit2)
    {
        int xDist = System.Math.Abs(unit1.Pos.y - unit2.Pos.y);
        int yDist = System.Math.Abs(unit1.Pos.x - unit2.Pos.x);
        return (xDist + yDist) == 1;
    }
}
