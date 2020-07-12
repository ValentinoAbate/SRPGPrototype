using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : GridObject
{
    public enum PColor
    { 
        White,
        Pink,
        Blue,
        Green,
    }

    [System.Flags]
    public enum Attributes
    { 
        None = 0,
        Fixed = 1,
    }

    [SerializeField]
    private string displayName;
    public string DisplayName => displayName;

    public TileUI.Type tileType;

    public PColor colors;
    public Attributes attributes;
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
