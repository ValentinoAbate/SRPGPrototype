using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Program : GridObject, ILootable, IHasKey
{
    public const string actionOnlyDescription = "Action";
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

    public int Id { get; private set; }
    public string Key => key;
    [SerializeField] private string key;

    public bool IsFusion => isFusion;
    [SerializeField] private bool isFusion = false;

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
    public void SetModifiers(IReadOnlyList<ProgramModifier> newModifiers)
    {
        modifiers = new ProgramModifier[newModifiers.Count];
        for (int i = 0; i < newModifiers.Count; i++)
        {
            modifiers[i] = newModifiers[i];
        }
    }
    // Effect Properties
    public ProgramEffect[] Effects => IsUpgraded ? Upgrade.ProgramEffects : effects;
    [SerializeField] private ProgramEffect[] effects;
    public void SetEffects(IReadOnlyList<ProgramEffect> newEffects, bool initialize)
    {
        effects = new ProgramEffect[newEffects.Count];
        for (int i = 0; i < newEffects.Count; i++)
        {
            effects[i] = newEffects[i];
            if (initialize)
            {
                effects[i].Initialize(this);
            }
        }
    }
    // Trigger / upgrade properties
    public ProgramUpgrade Upgrade { get; set; }
    public bool IsUpgraded => Upgrade != null;
    public IReadOnlyList<ProgramUpgrade> Upgrades => IsUpgraded ? Upgrade.Upgrades : upgrades;
    private List<ProgramUpgrade> upgrades;
        
    public string DisplayName => IsUpgraded ? Upgrade.DisplayName : displayName;
    [SerializeField] private string displayName = string.Empty;
    public string SetDisplayName(string newDisplayName) => displayName = newDisplayName;

    public string Description => IsUpgraded ? Upgrade.Description : description;
    [SerializeField] [TextArea(2,4)] private string description = string.Empty;
    public string SetDescription(string newDescription) => description = newDescription;

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

    public Program FusionArg1 { get; set; }
    public Program FusionArg2 { get; set; }

    public Program.Color color;
    public Attributes attributes;
    public Pattern shape;

    [SerializeField] private ProgramVariant[] variants;

    private readonly List<TileUI.Entry> uiEntries = new List<TileUI.Entry>();

    private void Awake()
    {
        Id = PersistantData.main.NewId;
        var upgradeList = new List<ProgramUpgrade>();
        foreach(Transform t in transform)
        {
            upgradeList.AddRange(t.GetComponents<ProgramUpgrade>());
        }
        foreach(var upgrade in upgradeList)
        {
            upgrade.Initialize(this);
        }
        upgrades = upgradeList;
        foreach(var effect in effects)
        {
            effect.Initialize(this);
        }
    }

    public void ApplyVariants()
    {
        foreach (var variant in variants)
        {
            variant.ApplyVariant(this);
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

    public void SetActionNames(string actionName)
    {
        foreach(var effect in Effects)
        {
            if(effect is ProgramEffectAddAction addAction)
            {
                addAction.action.DisplayName = actionName;
            }
        }
    }

    private const int usesId = 0;
    private const int variantId = 1;
    private const int upgradeId = 2;
    public const int fusionId = 3;
    private const int effectId = 4;

    public SaveManager.ProgramData Save(bool isBattle, ref List<SaveManager.ProgramData> fArgs)
    {
        var data = new SaveManager.ProgramData()
        {
            id = Id,
            k = Key,
        };
        if (attributes.HasFlag(Attributes.Transient))
        {
            var transient = GetComponent<ProgramAttributeTransient>();
            if(transient != null)
            {
                data.AddData(usesId, transient.Uses.ToString());
            }
        }
        // Variants
        if(variants.Length > 0)
        {
            var variantData = new List<string>(variants.Length);
            foreach (var variant in variants)
            {
                variantData.Add(variant.Save());
            }
            data.AddData(variantId, variantData);
        }
        data.u = IsUpgraded ? Upgrade.Key : string.Empty;
        if(Upgrades.Count > 0)
        {
            var upgradeData = new List<string>(Upgrades.Count);
            foreach(var upgrade in Upgrades)
            {
                upgradeData.Add(upgrade.Save());
            }
            data.AddData(upgradeId, upgradeData);
        }
        if(FusionArg1 != null && FusionArg2 != null)
        {
            var p1 = FusionArg1.Save(isBattle, ref fArgs);
            var p2 = FusionArg2.Save(isBattle, ref fArgs);
            fArgs.Add(p1);
            fArgs.Add(p2);
            data.AddData(fusionId, p1.id.ToString(), p2.id.ToString(), DisplayName, shape.Save());
        }
        var currentEffects = Effects;
        if (currentEffects.Length > 0)
        {
            List<string> effectData = null;
            for (int i = 0; i < currentEffects.Length; i++)
            {
                var effect = currentEffects[i];
                if (effect.CanSave(isBattle))
                {
                    effectData ??= new List<string>(currentEffects.Length - i);
                    effectData.Add(effect.Save(isBattle));
                }
            }
            if(effectData != null)
            {
                data.AddData(effectId, effectData);
            }
        }
        return data;
    }

    public void Load(SaveManager.ProgramData programData, bool isBattle, Unit unit)
    {
        Id = programData.id;
        if (!string.IsNullOrEmpty(programData.u))
        {
            LoadUpgrade(programData.u);
        }
        foreach(var data in programData.d)
        {
            var type = data.t;
            if(type == usesId)
            {
                if(data.Count > 0 && int.TryParse(data[0], out int uses))
                {
                    var transient = GetComponent<ProgramAttributeTransient>();
                    if (transient != null)
                    {
                        transient.Uses = uses;
                    }
                }
            }
            else if(type == variantId)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (i >= variants.Length)
                        break;
                    variants[i].Load(this, data[i]);
                }
            }
            else if(type == upgradeId)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (i >= Upgrades.Count)
                        break;
                    Upgrades[i].Load(data[i]);
                }
            }
            else if(type == effectId)
            {
                if(data.Count > 0)
                {
                    int index = 0;
                    foreach (var effect in Effects)
                    {
                        if (effect.CanSave(isBattle))
                        {
                            effect.Load(data[index++], isBattle, unit);
                            if (index >= data.Count)
                                break;
                        }
                    }
                }
            }
        }
    }

    private void LoadUpgrade(string upgradeKey)
    {
        foreach(var upgrade in upgrades)
        {
            if(LoadUpgradeRecursive(upgrade, upgradeKey))
            {
                return;
            }
        }
    }

    private bool LoadUpgradeRecursive(ProgramUpgrade upgrade, string upgradeKey)
    {
        if (upgrade.Key == upgradeKey)
        {
            Upgrade = upgrade;
            return true;
        }
        foreach (var u in upgrade.Upgrades)
        {
            if (LoadUpgradeRecursive(u, upgradeKey))
                return true;
        }
        return false;
    }

#if UNITY_EDITOR
    public void LinkComponents()
    {
        modifiers = GetComponents<ProgramModifier>();
        effects = GetComponents<ProgramEffect>();
        variants = GetComponents<ProgramVariant>();
    }

    private const string removeWord = "Program";
    public bool GenerateKey()
    {
        if (string.IsNullOrEmpty(key))
        {
            key = name.Replace(removeWord, string.Empty);
            return true;
        }
        return false;
    }
#endif
}
