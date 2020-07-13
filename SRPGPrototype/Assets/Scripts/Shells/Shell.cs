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
    
    public void Install(Program program, Vector2Int location)
    {
        var prog = Instantiate(program.gameObject, transform).GetComponent<Program>();
        programs.Add(new InstalledProgram(prog, location));
    }

    public void Uninstall(Program program, Vector2Int location)
    {
        var ind = programs.FindIndex((iProg) => iProg.program == program && iProg.location == location);
        if (ind >= 0)
        {
            Destroy(programs[ind].program.gameObject);
            programs.RemoveAt(ind);
        }
    }

    private void Awake()
    {
        foreach(var iProg in preInstalledPrograms)
        {
            Install(iProg.program, iProg.location);
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
