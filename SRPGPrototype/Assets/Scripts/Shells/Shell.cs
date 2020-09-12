using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shell : MonoBehaviour, ILootable
{
    public enum Progression
    { 
        Small,
        Standard,
        Large,
        Full,
    }

    public delegate bool Restriction(Shell shell, out string errorMessage);

    private static readonly Dictionary<Progression, List<int>> levelThresholds = new Dictionary<Progression, List<int>>()
    {
        { Progression.Small, new List<int>{ 0, 1, 4} },
        { Progression.Standard, new List<int>{ 0, 2, 5} },
        { Progression.Large, new List<int>{ 0, 3, 6} },
        { Progression.Full, new List<int>{ 0, 2, 4, 6, 8} },
    };

    public int MaxLevel => CapacityThresholds.Count - 1;

    public List<int> CapacityThresholds => levelThresholds[Type];

    public int Level { get; private set; } = 0;

    public int Capacity { get; private set; } = 0;

    public bool Compiled { get; private set; } = false;

    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public string Description => description;
    [SerializeField] [TextArea(1, 2)] private string description = string.Empty;

    public Rarity Rarity => rarity;
    [SerializeField] private Rarity rarity = Rarity.PreInstall;

    public Progression Type => type;
    [SerializeField] private Progression type = Progression.Standard;

    public Pattern CustArea => patterns[Level];
    [SerializeField] private List<Pattern> patterns = new List<Pattern>(3);

    public List<InstalledProgram> preInstalledPrograms = new List<InstalledProgram>();
    public IEnumerable<InstalledProgram> Programs => programs;
    private readonly List<InstalledProgram> programs = new List<InstalledProgram>();

    public IEnumerable<Action> Actions => actions;
    private readonly List<Action> actions = new List<Action>();

    public Unit.OnAfterSubAction AbilityOnAfterSubAction { get; private set; }
    public Unit.OnDeath AbilityOnDeath { get; private set; }
    public Unit.OnBattleStartDel AbilityOnBattleStart { get; private set; }

    public Stats Stats { get; set; } = new Stats();

    public Program[,] InstallMap { get; private set; }

    public bool HasSoulCore => SoulCoreUnitPrefab != null;
    public GameObject SoulCoreUnitPrefab { get; private set; }

    private bool firstCompile = true;

    private void Awake()
    {
        InstallMap = new Program[CustArea.Dimensions.x, CustArea.Dimensions.y];
        foreach (var iProg in preInstalledPrograms)
        {
            Install(iProg.program, iProg.location, true);
        }
    }

    public void Install(Program program, Vector2Int location, bool fromAsset = false)
    {
        if(fromAsset)
        {
            var prog = Instantiate(program.gameObject, transform).GetComponent<Program>();
            programs.Add(new InstalledProgram(prog, location));
            prog.Shell = this;
        }
        else
        {
            program.transform.SetParent(transform);
            programs.Add(new InstalledProgram(program, location));
            program.Shell = this;
        }
        var positions = program.shape.OffsetsShifted(location, false);
        foreach (var pos in positions)
            InstallMap[pos.x, pos.y] = program;
        Compiled = false;
    }

    public void Uninstall(Program program, Vector2Int location, bool destroy = false)
    {
        if (program.Shell != this)
            return;
        program.Shell = null;
        var ind = programs.FindIndex((iProg) => iProg.program.DisplayName == program.DisplayName && iProg.location == location);
        if (ind >= 0)
        {
            var positions = program.shape.OffsetsShifted(location, false);
            foreach (var pos in positions)
                InstallMap[pos.x, pos.y] = null;
            if (destroy)
                Destroy(programs[ind].program.gameObject);
            programs.RemoveAt(ind);
            Compiled = false;
        }
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
        var newInstallMap = new Program[CustArea.Dimensions.x + 1, CustArea.Dimensions.y + 1];
        // Level is even, expand into bottom right
        if (Level % 2 == 0)
        {
            for(int x = 0; x < CustArea.Dimensions.x; ++x)
                for(int y = 0; y < CustArea.Dimensions.y; ++y)
                    newInstallMap[x, y + 1] = InstallMap[x, y];
            for(int i = 0; i < programs.Count; ++i)
            {
                programs[i] = new InstalledProgram(programs[i].program, programs[i].location + new Vector2Int(0, 1));
                programs[i].program.Pos += new Vector2Int(0,1);
            }
        }
        else // Level is odd, expand into top left
        {
            for (int x = 0; x < CustArea.Dimensions.x; ++x)
                for (int y = 0; y < CustArea.Dimensions.y; ++y)
                    newInstallMap[x + 1, y] = InstallMap[x, y];
            for (int i = 0; i < programs.Count; ++i)
            {
                programs[i] = new InstalledProgram(programs[i].program, programs[i].location + new Vector2Int(1, 0));
                programs[i].program.Pos += new Vector2Int(1, 0);
            }
        }
        InstallMap = newInstallMap;
        ++Level;
        Compiled = false;
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
        var newInstallMap = new Program[CustArea.Dimensions.x - 1, CustArea.Dimensions.y - 1];
        // Level is odd, contract from bottom right
        if (Level % 2 == 1)
        {
            for (int x = 0; x < CustArea.Dimensions.x - 1; ++x)
                for (int y = 1; y < CustArea.Dimensions.y; ++y)
                    newInstallMap[x, y - 1] = InstallMap[x, y];
            for (int i = 0; i < programs.Count; ++i)
            {
                programs[i] = new InstalledProgram(programs[i].program, programs[i].location + new Vector2Int(0, -1));
                programs[i].program.Pos += new Vector2Int(0, -1);
            }
        }
        else // Level is even, contract from top left
        {
            for (int x = 1; x < CustArea.Dimensions.x; ++x)
                for (int y = 0; y < CustArea.Dimensions.y - 1; ++y)
                    newInstallMap[x - 1, y] = InstallMap[x, y];
            for (int i = 0; i < programs.Count; ++i)
            {
                programs[i] = new InstalledProgram(programs[i].program, programs[i].location + new Vector2Int(-1, 0));
                programs[i].program.Pos += new Vector2Int(-1, 0);
            }
        }
        InstallMap = newInstallMap;
        --Level;
        Compiled = false;
    }

    #endregion

    public CompileData GetCompileData(out List<Action> newActions)
    {
        newActions = new List<Action>();
        var compileData = new CompileData(new Stats(), new List<Action>(), new List<Restriction>());
        // Look through programs and apply program effects
        foreach (var install in programs)
        {
            foreach (var effect in install.program.Effects)
            {
                compileData.actions.Clear();
                effect.ApplyEffect(install.program, ref compileData);
                // Instantiate new actions
                foreach (var action in compileData.actions)
                {
                    var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
                    actionInstance.Program = install.program;
                    newActions.Add(actionInstance);
                }
            }
        }
        return compileData;
    }

    /// <summary>
    /// Compiles the stats and abilities from the programs in the shell, and outputs them.
    /// Will become more complicated, with adjacency, etc. later
    /// Returns false if the compile in considered invalid (either because of a compile rule or MaxHp <= 0)
    /// </summary>
    public bool Compile()
    {
        // Generate the compile data
        var compileData = GetCompileData(out List<Action> newActions);
        // Check Max Hp
        if (compileData.stats.MaxHP <= 0)
        {
            Debug.LogWarning("Compile Error: Max Hp <= 0");
            newActions.ForEach((a) => Destroy(a.gameObject));
            Compiled = false;
            return false;
        }
        // Check number of actions
        if(newActions.Count <= 0)
        {
            Debug.LogWarning("Compile Error: No Actions");
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
            if(restriction(this, out string errorMessage))
            {
                Debug.LogWarning(errorMessage);
                newActions.ForEach((a) => Destroy(a.gameObject));
                Compiled = false;
                return false;
            }
        }
        // Check num soul cores
        if(programs.Count((p) => p.program.attributes.HasFlag(Program.Attributes.SoulCore)) > 1)
        {
            Debug.LogWarning("Compile Error: More than one soul core installed");
            Compiled = false;
            return false;
        }
        // Apply Soul Cores
        SoulCoreUnitPrefab = compileData.soulCoreUnitPrefab;
        // Apply capacity
        Capacity = compileData.capacity;
        // Apply abilities
        AbilityOnAfterSubAction = compileData.abilityOnAfterSubAction;
        AbilityOnDeath = compileData.abilityOnDeath;
        AbilityOnBattleStart = compileData.abilityOnBattleStart;
        // Apply compile changes to actions and stats
        Stats.SetShellValues(compileData.stats);
        // Restore all HP if first compile
        if(firstCompile)
        {
            Stats.RestoreHpToMax();
            firstCompile = false;
        }
        // Copy Temporary values of already instantiated actions to their newly generated copies
        foreach(var action in actions)
        {
            var sameAction = newActions.Find((a) => action.DisplayName == a.DisplayName && action.Program == a.Program);
            if (sameAction != null)
                sameAction.CopyTemporaryValues(action);
            Destroy(action.gameObject);
        }
        actions.Clear();
        actions.AddRange(newActions);
        Compiled = true;
        return true;
    }

    public ref struct CompileData
    {
        public Stats stats;
        public List<Action> actions;
        public List<Restriction> restrictions;
        public Unit.OnAfterSubAction abilityOnAfterSubAction;
        public Unit.OnDeath abilityOnDeath;
        public Unit.OnBattleStartDel abilityOnBattleStart;
        public GameObject soulCoreUnitPrefab;
        public int capacity;
        public CompileData(Stats stats, List<Action> actions, List<Restriction> restrictions)
        {
            this.stats = stats;
            this.actions = actions;
            this.restrictions = restrictions;
            abilityOnAfterSubAction = null;
            abilityOnDeath = null;
            abilityOnBattleStart = null;
            soulCoreUnitPrefab = null;
            capacity = 0;
        }
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
}
