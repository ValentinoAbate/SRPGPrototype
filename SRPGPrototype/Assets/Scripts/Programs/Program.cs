using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Program : GridObject, ILootable
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
        Transient = 2,
    }

    public Shell Shell { get; set; }

    public ProgramEffect[] Effects => GetComponents<ProgramEffect>();
    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public string Description => description;
    [SerializeField] [TextArea(2,4)] private string description = string.Empty;

    public Rarity Rarity => rarity;
    [SerializeField] Rarity rarity = Rarity.Common;

    public TileUI.Type tileType;

    public PColor colors;
    public Attributes attributes;
    public Pattern shape;

    private List<TileUI.Entry> uiEntries = new List<TileUI.Entry>();

    public void Show(Vector2Int pos, CustGrid grid)
    {
        uiEntries.Clear();
        foreach (var shiftPos in shape.OffsetsShifted(pos))
        {
            uiEntries.Add(grid.SpawnTileUI(shiftPos, tileType));
        }
        if (attributes.HasFlag(Attributes.Fixed))
        {
            foreach(var entry in uiEntries)
            {
                entry.obj.GetComponent<MeshRenderer>().material.SetFloat("BoolFixed", 1);
            }
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
