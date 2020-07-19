using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shell : MonoBehaviour, ILootable
{
    public delegate bool Restriction(Shell shell, out string errorMessage);
    public bool Compiled { get; private set; } = false;

    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public Rarity Rarity => rarity;
    [SerializeField] private Rarity rarity = Rarity.PreInstall;

    public Pattern custArea = null;
    public List<InstalledProgram> preInstalledPrograms = new List<InstalledProgram>();
    public IEnumerable<InstalledProgram> Programs => programs;
    private List<InstalledProgram> programs = new List<InstalledProgram>();

    public IEnumerable<Action> Actions => actions;
    private List<Action> actions = new List<Action>();

    public Stats Stats { get; set; } = new Stats();

    [HideInInspector] public Program[,] installMap;

    private void Awake()
    {
        installMap = new Program[custArea.Dimensions.x, custArea.Dimensions.y];
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
        var positions = program.shape.OffsetsShifted(location);
        foreach (var pos in positions)
            installMap[pos.x, pos.y] = program;
    }

    public void Uninstall(Program program, Vector2Int location, bool destroy = false)
    {
        if (program.Shell != this)
            return;
        program.Shell = null;
        var ind = programs.FindIndex((iProg) => iProg.program.DisplayName == program.DisplayName && iProg.location == location);
        if (ind >= 0)
        {
            var positions = program.shape.OffsetsShifted(location);
            foreach (var pos in positions)
                installMap[pos.x, pos.y] = null;
            if (destroy)
                Destroy(programs[ind].program.gameObject);
            programs.RemoveAt(ind);
        }
    }

    /// <summary>
    /// Compiles the stats and abilities from the programs in the shell, and outputs them.
    /// Will become more complicated, with adjacency, etc. later
    /// Returns false if the compile in considered invalid (either because of a compile rule or MaxHp <= 0)
    /// </summary>
    public bool Compile()
    {
        var newActions = new List<Action>();
        var compileData = new CompileData(new Stats(), new List<Action>(), new List<Restriction>());
        // Look through programs and apply program effects
        foreach(var install in programs)
        {
            foreach(var effect in install.program.Effects)
            {
                compileData.actions.Clear();
                effect.ApplyEffect(install.program, ref compileData);
                // Instantiate new actions
                foreach(var action in compileData.actions)
                {
                    var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
                    actionInstance.Program = install.program;
                    newActions.Add(actionInstance);
                }
            }
        }
        // Check Max Hp
        if(compileData.stats.MaxHp <= 0)
        {
            Debug.LogWarning("Compile Error: Max Hp <= 0");
            newActions.ForEach((a) => Destroy(a.gameObject));
            Compiled = false;
            return false;
        }
        // Check restirctions
        foreach(var restriction in compileData.restrictions)
        {
            if(restriction(this, out string errorMessage))
            {
                Debug.LogWarning(errorMessage);
                newActions.ForEach((a) => Destroy(a.gameObject));
                Compiled = false;
                return false;
            }
        }
        // Apply compile changes to actions and stats
        Stats.SetShellValues(compileData.stats);
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

    public struct CompileData
    {
        public Stats stats;
        public List<Action> actions;
        public List<Restriction> restrictions;
        public CompileData(Stats stats, List<Action> actions, List<Restriction> restrictions)
        {
            this.stats = stats;
            this.actions = actions;
            this.restrictions = restrictions;
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

}
