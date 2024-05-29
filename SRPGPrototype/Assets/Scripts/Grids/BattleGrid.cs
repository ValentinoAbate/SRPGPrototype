using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : Grid.Grid<Unit>
{
    [SerializeField] private Vector2Int dimensions = new Vector2Int(8, 8);
    public override Vector2Int Dimensions => dimensions;

    public System.Action<Unit> OnAddUnit { get; set; }

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
            OnAddUnit?.Invoke(obj);
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
}
