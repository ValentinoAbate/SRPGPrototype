﻿using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Program : GridObject, ILootable
{
    private const string highlightTileUIProp = "BoolHighlight";
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
        Gamble = 8,
        Starter = 16,
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
    public UnityEngine.Color ContrastColorValue => color == Color.Blue ? UnityEngine.Color.white : UnityEngine.Color.black;

    public Shell Shell { get; set; }
    // Modifier properties
    public IReadOnlyList<Modifier> ModifiedBy
    {
        get
        {
            if (Shell == null)
            {
                return System.Array.Empty<Modifier>();
            }
            var modifierMap = Shell.Compiled ? Shell.ModifierMap : Shell.LatestCompileData?.modifierMap;
            if (modifierMap != null && modifierMap.ContainsKey(this))
            {
                return modifierMap[this];
            }
            return System.Array.Empty<Modifier>();
        }
    }

    public IEnumerable<T> ModifiedByType<T>() where T : Modifier
    {
        foreach(var m in ModifiedBy)
        {
            if(m is T t)
            {
                yield return t;
            }
        }
    }
    public ProgramModifier[] ModifierEffects => IsUpgraded ? Upgrade.ModifierEffects : modifiers;
    [SerializeField] private ProgramModifier[] modifiers;
    // Effect Properties
    public ProgramEffect[] Effects => IsUpgraded ? Upgrade.ProgramEffects : effects;
    [SerializeField] private ProgramEffect[] effects;
    // Trigger / upgrade properties
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
            if (attributes.HasFlag(Attributes.Gamble))
            {
                attTexts.Add("Gamble");
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
            return string.Join(", ", attTexts);
        }
    }

    public string ModifiedByText
    {
        get
        {
            var mods = ModifiedBy;
            if (mods.Count <= 0)
                return string.Empty;
            if (mods.Count == 1)
                return mods[0].DisplayName;
            var text = new string[mods.Count];
            for (int i = 0; i < mods.Count; i++)
            {
                text[i] = mods[i].DisplayName;
            }
            return string.Join(" ", text);
        }
    }

    public string FullName
    {
        get
        {
            var modText = ModifiedByText;
            if (string.IsNullOrEmpty(modText))
                return DisplayName;
            return $"{DisplayName} {modText}";
        }
    }

    public Program.Color color;
    public Attributes attributes;
    public Pattern shape;

    private readonly List<TileUI.Entry> uiEntries = new List<TileUI.Entry>();

    private void Awake()
    {
        var trigList = new List<ProgramTrigger>();
        foreach(Transform t in transform)
        {
            trigList.AddRange(t.GetComponents<ProgramTrigger>());
        }
        foreach(var trigger in trigList)
        {
            trigger.Initialize(this);
        }
        triggers = trigList.ToArray();
        foreach(var effect in effects)
        {
            effect.Initialize(this);
        }
    }

    public void Highlight()
    {
        foreach (var entry in uiEntries)
        {
            entry.obj.GetComponent<MeshRenderer>().material.SetFloat(highlightTileUIProp, 1);
        }
    }

    public void UnHighlight()
    {
        foreach (var entry in uiEntries)
        {
            entry.obj.GetComponent<MeshRenderer>().material.SetFloat(highlightTileUIProp, 0);
        }
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

#if UNITY_EDITOR
    public void SetEffects()
    {
        modifiers = GetComponents<ProgramModifier>();
        effects = GetComponents<ProgramEffect>();
    }
#endif
}
