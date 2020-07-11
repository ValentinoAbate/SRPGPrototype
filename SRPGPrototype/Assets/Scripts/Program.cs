using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : GridObject
{
    public enum ProgColor
    { 
        White,
        Pink,
        Blue,
        Green,
    }
    public TileUI.Type tileType;

    public ProgColor colors;
    public Pattern shape;
    private List<TileUI.Entry> uiEntries = new List<TileUI.Entry>();

    public void Show(Vector2Int pos, CustGrid grid)
    {
        foreach (var shiftPos in shape.OffsetsShifted(pos))
        {
            uiEntries.Add(grid.SpawnTileUI(shiftPos, tileType));
        }
    }

    public void Hide(CustGrid grid)
    {
        foreach (var entry in uiEntries)
        {
            grid.RemoveTileUI(entry);
        }
        uiEntries.Clear();
    }
}
