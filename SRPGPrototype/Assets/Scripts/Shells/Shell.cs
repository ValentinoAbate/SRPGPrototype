using Extensions.VectorIntDimensionUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shell : MonoBehaviour, ILootable
{
    public const int baseLinkOutThreshold = 2;
    public enum Progression
    {
        Small,
        Standard,
        Large,
        Full,
    }

    public delegate bool Restriction(Shell shell, out string errorMessage);

    public delegate void OnProgramDestroyedDel(Program p, BattleGrid grid, Unit user);

    private static readonly Dictionary<Progression, List<int>> levelThresholds = new Dictionary<Progression, List<int>>()
    {
        { Progression.Small, new List<int>{ 0, 1, 4} },
        { Progression.Standard, new List<int>{ 0, 2, 5} },
        { Progression.Large, new List<int>{ 0, 3, 6} },
        { Progression.Full, new List<int>{ 0, 2, 4, 6, 8} },
    };

    public int Id { get; private set; }
    public string Key => key;
    [SerializeField] private string key;

    public int MaxLevel => CapacityThresholds.Count - 1;

    public List<int> CapacityThresholds => levelThresholds[Type];

    public int DisplayLevel => Level + 1;
    public int Level { get; private set; } = 0;

    public int Capacity { get; private set; } = 0;

    public bool Compiled { get; private set; } = false;

    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public string Description => description;
    [SerializeField] [TextArea(1, 2)] private string description = string.Empty;

    // Loot Properties

    public Rarity Rarity => rarity;
    [SerializeField] private Rarity rarity = Rarity.PreInstall;
    public float LootWeight => lootWeight;
    [SerializeField] float lootWeight = 1;

    public Progression Type => type;
    [SerializeField] private Progression type = Progression.Standard;

    public Pattern CustArea => patterns[Level];
    [SerializeField] private List<Pattern> patterns = new List<Pattern>(3);

    public List<InstalledProgram> preInstalledPrograms = new List<InstalledProgram>();
    public IReadOnlyList<InstalledProgram> Programs => programs;
    private readonly List<InstalledProgram> programs = new List<InstalledProgram>();

    public IEnumerable<Program> RawPrograms
    {
        get
        {
            foreach (var prog in programs)
            {
                yield return prog.program;
            }
        }
    }

    public IReadOnlyList<Action> Actions => actions;
    private readonly List<Action> actions = new List<Action>();

    public Unit.OnSubAction OnBeforeSubAction { get; private set; }
    public Unit.OnSubAction OnAfterSubAction { get; private set; }
    public Unit.OnAfterAction OnAfterAction { get; private set; }
    public Unit.OnDeath OnDeath { get; private set; }
    public Unit.OnDamaged OnDamaged { get; private set; }
    public Unit.IncomingDamageMod IncomingDamageMods { get; private set; }
    public Unit.OnRepositioned OnRepositioned { get; private set; }
    public Unit.OnRepositioned OnRepositionOther { get; private set; }
    public Unit.OnBattleStartDel OnBattleStart { get; private set; }
    public Unit.OnPhaseStartDel OnPhaseStart { get; private set; }
    public Unit.OnPhaseEndDel OnPhaseEnd { get; private set; }
    public OnProgramDestroyedDel OnProgramDestroyed { get; private set; }

    public Stats Stats { get; } = new Stats();
    public Dictionary<Program, List<Modifier>> ModifierMap { get; private set; }

    public Program[,] InstallMap { get; private set; }
    public Dictionary<Program, List<Vector2Int>> InstallPositions { get; } = new Dictionary<Program, List<Vector2Int>>();

    public bool HasSoulCore => SoulCoreUnitPrefab != null;
    public GameObject SoulCoreUnitPrefab { get; private set; }
    public int LinkOutThreshold { get; private set; } = baseLinkOutThreshold;
    // Current compile data. May not reflect a shell that can actually compile
    public CompileData LatestCompileData { get; private set; }

    public int SaveId { get; private set; } = 0;

    private void Awake()
    {
        Id = PersistantData.main.NewId;
        InstallMap = new Program[CustArea.Dimensions.x, CustArea.Dimensions.y];
        var positionsSet = CustArea.OffsetsSet;
        foreach (var install in preInstalledPrograms)
        {
            Install(install.program, install.location, true);
            var positions = install.program.shape.OffsetsShifted(install.location, false);
            foreach (var pos in positions)
            {
                if (!positionsSet.Contains(pos))
                {
                    patterns[0].AddOffset(pos);
                    patterns[1].AddOffset(new Vector2Int(pos.x, pos.y + 1));
                    patterns[2].AddOffset(new Vector2Int(pos.x + 1, pos.y + 1));
                }
            }
        }
        // Set initial stats
        var compileData = GetCompileData();
        Stats.SetShellValues(compileData.stats);
        Stats.RestoreHpToMax();
    }

    public void Install(Program program, Vector2Int location, bool fromAsset = false)
    {
        Program prog;
        if (fromAsset)
        {
            prog = Instantiate(program.gameObject, transform).GetComponent<Program>();
        }
        else
        {
            prog = program;
            program.transform.SetParent(transform);
        }
        programs.Add(new InstalledProgram(prog, location));
        prog.Shell = this;
        var positions = new List<Vector2Int>(prog.shape.OffsetsShifted(location, false));
        foreach (var pos in positions)
        {
            InstallMap[pos.x, pos.y] = prog;
        }
        InstallPositions.Add(prog, positions);
        Compiled = false;
    }

    public void Uninstall(Program program)
    {
        Uninstall(program, false);
    }

    private void Uninstall(Program program, bool destroy, BattleGrid grid = null, Unit user = null)
    {
        if (program.Shell != this)
            return;
        // Confirm program is actually in shell and record index for later removal
        int index = GetProgramIndex(program);
        if (index == -1)
            return;
        program.Shell = null;
        var positions = program.shape.OffsetsShifted(program.Pos, false);
        foreach (var pos in positions)
            InstallMap[pos.x, pos.y] = null;
        InstallPositions.Remove(program);
        if (destroy)
        {
            OnProgramDestroyed?.Invoke(program, grid, user);
            Destroy(program.gameObject);
        }
        programs.RemoveAt(index);
        Compiled = false;
    }

    public void DestroyProgram(Program program, BattleGrid grid, Unit user)
    {
        Uninstall(program, true, grid, user);
        if (!Compile())
        {
            Debug.LogError("Destroyed program has caused shell compile error");
        }
    }

    public Program GetProgram(Vector2Int location)
    {
        if (!location.IsBoundedBy(CustArea.Dimensions))
            return null;
        return InstallMap[location.x, location.y];
    }

    private int GetProgramIndex(Program program)
    {
        for (int i = 0; i < programs.Count; i++)
        {
            if (programs[i].program == program)
            {
                return i;
            }
        }
        return -1;
    }

    #region Leveling Code

    public void SetLevel(int targetLevel)
    {
        if (targetLevel < 0 || targetLevel > MaxLevel)
            return;
        while (Level != targetLevel)
        {
            if (Level > targetLevel)
                LevelDown();
            else
                LevelUp();
        }
    }

    public void LevelUp()
    {
        if (Level >= MaxLevel)
            return;
        // Level is even, expand into bottom right
        if (Level % 2 == 0)
        {
            ChangeCustArea(Vector2Int.one, new Vector2Int(1, -1));
        }
        else // Level is odd, expand into top left
        {
            ChangeCustArea(Vector2Int.one, new Vector2Int(-1, 1));
        }
        ++Level;
        Compiled = false;
    }

    private void ChangeCustArea(Vector2Int sizeDelta, Vector2Int deltaDirection)
    {
        // Create expanded install map
        var newInstallMap = new Program[CustArea.Dimensions.x + sizeDelta.x, CustArea.Dimensions.y + sizeDelta.y];
        // Copy shifted install positions from old install map
        var shiftBy = Vector2Int.zero;
        if (deltaDirection.x < 0)
        {
            shiftBy.x = sizeDelta.x;
        }
        if (deltaDirection.y < 0)
        {
            shiftBy.y = sizeDelta.y;
        }
        for (int x = -Math.Min(shiftBy.x, 0); x < CustArea.Dimensions.x + Math.Min(shiftBy.y, 0); ++x)
            for (int y = -Math.Min(shiftBy.y, 0); y < CustArea.Dimensions.y + Math.Min(shiftBy.x, 0); ++y)
                newInstallMap[x + shiftBy.x, y + shiftBy.y] = InstallMap[x, y];
        // Overwrite install map
        InstallMap = newInstallMap;
        // Shift programs
        for (int i = 0; i < programs.Count; ++i)
        {
            programs[i] = new InstalledProgram(programs[i].program, programs[i].location + shiftBy);
            programs[i].program.Pos += shiftBy;
        }
        // Shift install positions
        var newPositions = new List<Vector2Int>();
        foreach (var kvp in InstallPositions)
        {
            newPositions.Clear();
            foreach (var pos in kvp.Value)
            {
                newPositions.Add(pos + shiftBy);
            }
            kvp.Value.Clear();
            kvp.Value.AddRange(newPositions);
        }
    }

    public bool CanLevelDown()
    {
        if (Level <= 0)
            return false;
        // Level is odd, check bottom row and right column
        if (Level % 2 == 1)
        {
            // Bottom Row
            for (int x = 0; x < CustArea.Dimensions.x; ++x)
                if (InstallMap[x, 0] != null)
                    return false;
            // Left Column
            for (int y = 0; y < CustArea.Dimensions.y; ++y)
                if (InstallMap[CustArea.Dimensions.x - 1, y] != null)
                    return false;
        }
        else // Level is even, check top row and left column
        {
            // Top Row
            for (int x = 0; x < CustArea.Dimensions.x; ++x)
                if (InstallMap[x, CustArea.Dimensions.y - 1] != null)
                    return false;
            // Left Column
            for (int y = 0; y < CustArea.Dimensions.y; ++y)
                if (InstallMap[0, y] != null)
                    return false;
        }
        return true;
    }

    public void LevelDown()
    {
        if (Level <= 0)
            return;
        // Level is odd, contract from bottom right
        if (Level % 2 == 1)
        {
            ChangeCustArea(-Vector2Int.one, new Vector2Int(1, -1));
        }
        else // Level is even, contract from top left
        {
            ChangeCustArea(-Vector2Int.one, new Vector2Int(-1, 1));
        }
        --Level;
        Compiled = false;
    }

    #endregion

    public Dictionary<Program, List<Modifier>> GetModifierMap()
    {
        var modMap = new Dictionary<Program, List<Modifier>>();
        // If the shell is an asset, generate compile data from pre-installs
        var programList = InstallMap == null ? preInstalledPrograms : programs;
        // Look through programs and apply program effects
        foreach (var install in programList)
        {
            var program = install.program;
            foreach (var mod in program.ModifierEffects)
            {
                mod.LinkModifiers(program, ref modMap);
            }
        }
        return modMap;
    }

    public CompileData GetCompileData()
    {
        var compileData = new CompileData
        {
            modifierMap = GetModifierMap()
        };
        // If the shell is an asset, generate compile data from pre-installs
        var programList = InstallMap == null ? preInstalledPrograms : programs;
        // Look through programs and apply program effects
        foreach (var install in programList)
        {
            var program = install.program;
            foreach (var effect in program.Effects)
            {
                effect.ApplyEffect(ref compileData);
            }
            foreach (var trigger in program.Triggers)
            {
                trigger.Condition.LinkEffect(program, ref compileData);
            }
        }
        LatestCompileData = compileData;
        return compileData;
    }

    /// <summary>
    /// Compiles the stats and abilities from the programs in the shell, and outputs them.
    /// Will become more complicated, with adjacency, etc. later
    /// Returns false if the compile in considered invalid (compile error)
    /// </summary>
    public bool Compile()
    {
        // Generate the compile data
        var compileData = GetCompileData();
        // Check Max Hp
        if (compileData.stats.MaxHP <= 0)
        {
            Debug.LogWarning("Compile Error: Max Hp <= 0");
            Compiled = false;
            return false;
        }
        // Check capacity
        if (compileData.capacity < CapacityThresholds[Level])
        {
            Debug.LogWarning("Compile Error: Not enough capacity");
            Compiled = false;
            return false;
        }
        // Check restirctions
        foreach (var restriction in compileData.restrictions)
        {
            if (restriction(this, out string errorMessage))
            {
                Debug.LogWarning(errorMessage);
                Compiled = false;
                return false;
            }
        }
        // Check num soul cores
        if (programs.Count(IsInstallSoulCore) > 1)
        {
            Debug.LogWarning("Compile Error: More than one soul core installed");
            Compiled = false;
            return false;
        }
        // Apply modification map
        ModifierMap = compileData.modifierMap;
        // Apply Soul Cores
        SoulCoreUnitPrefab = compileData.soulCoreUnitPrefab;
        // Apply capacity
        Capacity = compileData.capacity;
        // Apply abilities
        OnAfterSubAction = compileData.onAfterSubAction;
        OnBeforeSubAction = compileData.onBeforeSubAction;
        OnAfterAction = compileData.onAfterAction;
        OnDeath = compileData.onDeath;
        OnDamaged = compileData.onDamaged;
        IncomingDamageMods = compileData.incomingDamageMods;
        OnRepositioned = compileData.onRepositioned;
        OnRepositionOther = compileData.onRepositionOther;
        OnBattleStart = compileData.onBattleStart;
        OnPhaseStart = compileData.onPhaseStart;
        OnPhaseEnd = compileData.onPhaseEnd;
        OnProgramDestroyed = compileData.onProgramDestroyed;
        // Apply compiled changes to actions and stats
        Stats.SetShellValues(compileData.stats);
        actions.Clear();
        actions.AddRange(compileData.actions);
        Compiled = true;
        return true;
    }

    public SaveManager.ShellData Save()
    {
        var shellData = new SaveManager.ShellData()
        {
            id = Id,
            key = Key,
            hp = Stats.HP,
            level = Level,
            progs = new List<SaveManager.InstalledProgramData>(Programs.Count)
        };
        foreach (var prog in Programs)
        {
            shellData.progs.Add(new SaveManager.InstalledProgramData()
            {
                pos = prog.location,
                prog = prog.program.Save(),
            });
        }
        return shellData;
    }

    public void Load(SaveManager.ShellData data)
    {
        Id = data.id;
    }

    private static bool IsInstallSoulCore(InstalledProgram p) => ProgramFilters.IsSoulCore(p.program);

    public class CompileData
    {
        public Stats stats = new Stats();
        public List<Action> actions = new List<Action>();
        public List<string> restrictionNames = new List<string>();
        public List<Restriction> restrictions = new List<Restriction>();
        public List<string> abilityNames = new List<string>();
        public Unit.OnSubAction onBeforeSubAction = null;
        public Unit.OnSubAction onAfterSubAction = null;
        public Unit.OnAfterAction onAfterAction = null;
        public Unit.OnDeath onDeath = null;
        public Unit.OnDamaged onDamaged = null;
        public Unit.IncomingDamageMod incomingDamageMods = null;
        public Unit.OnRepositioned onRepositioned = null;
        public Unit.OnRepositioned onRepositionOther = null;
        public Unit.OnBattleStartDel onBattleStart = null;
        public Unit.OnPhaseStartDel onPhaseStart = null;
        public Unit.OnPhaseEndDel onPhaseEnd = null;
        public OnProgramDestroyedDel onProgramDestroyed = null;
        public GameObject soulCoreUnitPrefab = null;
        public Dictionary<Program, List<Modifier>> modifierMap = new Dictionary<Program, List<Modifier>>();
        public int capacity = 0;
    }


    [System.Serializable]
    public struct InstalledProgram
    {
        public Program program;
        public Vector2Int location;

        public InstalledProgram(Program program, Vector2Int location)
        {
            this.program = program;
            this.location = location;
        }
    }

    public class Preset
    {
        public const string defaultName = "Empty";
        public string DisplayName { get; set; } = defaultName;
        public List<InstalledProgram> Programs { get; set; } = new List<InstalledProgram>();
        public int Level { get; set; }
    }

#if UNITY_EDITOR
    public bool GenerateKey()
    {
        if (string.IsNullOrEmpty(key))
        {
            key = name.Replace("Shell", string.Empty);
            return true;
        }
        return false;
    }
#endif
}
