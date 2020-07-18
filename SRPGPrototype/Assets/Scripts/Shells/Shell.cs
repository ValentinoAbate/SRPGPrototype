using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public Pattern custArea = null;
    public List<InstalledProgram> preInstalledPrograms = new List<InstalledProgram>();
    public IEnumerable<InstalledProgram> AllPrograms => programs;
    private List<InstalledProgram> programs = new List<InstalledProgram>();

    public IEnumerable<Action> Actions => actions;
    private List<Action> actions = new List<Action>();

    public Stats Stats { get; set; } = new Stats();

    private void Awake()
    {
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

    }

    public void Uninstall(Program program, Vector2Int location, bool destroy = false)
    {
        if (program.Shell != this)
            return;
        program.Shell = null;
        var ind = programs.FindIndex((iProg) => iProg.program.DisplayName == program.DisplayName && iProg.location == location);
        if (ind >= 0)
        {
            if(destroy)
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
        var stats = new Stats();
        var newActions = new List<Action>();
        foreach(var install in programs)
        {
            foreach(var effect in install.program.Effects)
            {
                var addedActions = new List<Action>();
                effect.ApplyEffect(install.program, ref stats, ref addedActions);
                foreach(var action in addedActions)
                {
                    var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
                    actionInstance.Program = install.program;
                    newActions.Add(actionInstance);
                }
            }
        }
        if(stats.MaxHp <= 0)
        {
            Debug.LogWarning("Compile Error: Max Hp <= 0");
            newActions.ForEach((a) => Destroy(a.gameObject));
            return false;
        }
        // Apply compile changes to actions and stats
        Stats.SetShellValues(stats);
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
        return true;
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
