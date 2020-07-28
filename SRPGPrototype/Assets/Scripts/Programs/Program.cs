using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : GridObject, ILootable
{
    public enum Color
    { 
        White,
        Red,
        Blue,
        Green,
        Yellow,
    }

    [System.Flags]
    public enum Attributes
    { 
        None = 0,
        Fixed = 1,
        Transient = 2,
    }

    private static readonly Dictionary<Color, TileUI.Type> tileTypes = new Dictionary<Color, TileUI.Type>()
    {
        { Color.White, TileUI.Type.CustWhite },
        { Color.Red, TileUI.Type.CustRed },
        { Color.Blue, TileUI.Type.CustBlue },
        { Color.Green, TileUI.Type.CustGreen },
        { Color.Yellow, TileUI.Type.CustYellow },
    };

    public Shell Shell { get; set; }

    public ProgramEffect[] Effects => GetComponents<ProgramEffect>();
    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public string Description => description;
    [SerializeField] [TextArea(2,4)] private string description = string.Empty;

    public Rarity Rarity => rarity;
    [SerializeField] Rarity rarity = Rarity.Common;

    public TileUI.Type TileType => tileTypes[color];

    public Program.Color color;
    public Attributes attributes;
    public Pattern shape;

    private List<TileUI.Entry> uiEntries = new List<TileUI.Entry>();

    public void Show(Vector2Int pos, CustGrid grid)
    {
        uiEntries.Clear();
        foreach (var shiftPos in shape.OffsetsShifted(pos, false))
        {
            uiEntries.Add(grid.SpawnTileUI(shiftPos, TileType));
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
