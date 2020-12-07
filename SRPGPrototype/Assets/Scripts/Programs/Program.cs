using System.Collections.Generic;
using System.Linq;

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
        SoulCore = 4,
    }

    private static readonly Dictionary<Color, TileUI.Type> tileTypes = new Dictionary<Color, TileUI.Type>()
    {
        { Color.White, TileUI.Type.CustWhite },
        { Color.Red, TileUI.Type.CustRed },
        { Color.Blue, TileUI.Type.CustBlue },
        { Color.Green, TileUI.Type.CustGreen },
        { Color.Yellow, TileUI.Type.CustYellow },
    };

    public static readonly Dictionary<Color, UnityEngine.Color> colorValues = new Dictionary<Color, UnityEngine.Color>()
    {
        { Color.White, UnityEngine.Color.white },
        { Color.Red, UnityEngine.Color.red },
        //{ Color.Blue, new UnityEngine.Color(0, 0x37, 0xFF, 0xFF) },
        { Color.Blue, UnityEngine.Color.blue },
        //{ Color.Green, new UnityEngine.Color(0, 0xFF, 0x22, 0xFF) },
        { Color.Green, UnityEngine.Color.green },
        { Color.Yellow, new UnityEngine.Color(0xFB, 0xFF, 0, 0xFF) },
    };

    public UnityEngine.Color ColorValue => colorValues[color];

    public Shell Shell { get; set; }
    public List<ProgramModifier> Modifiers => Shell.ModifierMap.ContainsKey(this) ? Shell.ModifierMap[this] : new List<ProgramModifier>();
    public ProgramEffect[] Effects => IsUpgraded ? Upgrade.ProgramEffects : effects;
    private ProgramEffect[] effects;
    public ProgramUpgrade Upgrade { get; set; }
    public bool IsUpgraded => Upgrade != null;
    public ProgramTrigger[] Triggers => IsUpgraded ? Upgrade.Upgrades : triggers;
    private ProgramTrigger[] triggers;
        
    public string DisplayName => IsUpgraded ? Upgrade.DisplayName : displayName;
    [SerializeField] private string displayName = string.Empty;

    public string Description => IsUpgraded ? Upgrade.Description : description;
    [SerializeField] [TextArea(2,4)] private string description = string.Empty;

    public Rarity Rarity => rarity;
    [SerializeField] Rarity rarity = Rarity.Common;

    public float LootWeight => lootWeight;
    [SerializeField] float lootWeight = 1;

    public TileUI.Type TileType => tileTypes[color];

    public string AttributesText
    {
        get
        {
            var attTexts = new List<string>();
            if (attributes.HasFlag(Attributes.Fixed))
            {
                attTexts.Add("Fixed");
            }
            if (attributes.HasFlag(Attributes.SoulCore))
            {
                attTexts.Add("Soul Core");
            }
            if (attributes.HasFlag(Attributes.Transient))
            {
                var transientAttr = GetComponent<ProgramAttributeTransient>();
                string errorText = "Error: No Attribute Component found";
                attTexts.Add("Transient " + (transientAttr == null ? errorText : transientAttr.UsesLeft.ToString()));
            }
            if (attTexts.Count <= 0)
                return string.Empty;
            if (attTexts.Count == 1)
                return attTexts[0];
            return attTexts.Aggregate((s1, s2) => s1 + ", " + s2);
        }
    }

    public Program.Color color;
    public Attributes attributes;
    public Pattern shape;

    private readonly List<TileUI.Entry> uiEntries = new List<TileUI.Entry>();

    private void Awake()
    {
        triggers = GetComponentsInChildren<ProgramTrigger>();
        effects = GetComponents<ProgramEffect>();
    }

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
