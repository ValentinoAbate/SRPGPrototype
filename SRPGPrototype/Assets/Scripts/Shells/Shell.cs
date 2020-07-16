using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public Pattern custArea = null;
    public List<InstalledProgram> preInstalledPrograms = new List<InstalledProgram>();
    public IEnumerable<InstalledProgram> AllPrograms => programs;
    private List<InstalledProgram> programs = new List<InstalledProgram>();

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
        }
        else
        {
            program.transform.SetParent(transform);
            programs.Add(new InstalledProgram(program, location));
        }

    }

    public void Uninstall(Program program, Vector2Int location, bool destroy = false)
    {
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
    public bool Compile(out PlayerStats stats, out List<Player.ProgramAction> actions)
    {
        stats = new PlayerStats();
        actions = new List<Player.ProgramAction>();
        foreach(var install in programs)
        {
            foreach(var effect in install.program.Effects)
            {
                effect.ApplyEffect(install.program, ref stats, ref actions);
            }
        }
        if(stats.MaxHp <= 0)
        {
            Debug.LogWarning("Compile Error: Max Hp <= 0");
            return false;
        }
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
