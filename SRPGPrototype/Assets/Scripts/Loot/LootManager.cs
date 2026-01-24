using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public List<Program> programs = new List<Program>();
    public List<Shell> shells = new List<Shell>();

    public Loot<Shell> ShellLoot { get; private set; }
    public Loot<Program> ProgramLoot { get; private set; }
    public LootUI UI => lootUI;
    [SerializeField] private LootUI lootUI = null;

    private readonly Dictionary<string, Program> programLookup = new Dictionary<string, Program>();
    private readonly Dictionary<string, Shell> shellLookup = new Dictionary<string, Shell>();

    private void Awake()
    {
        ShellLoot = new Loot<Shell>(shells);
        ProgramLoot = new Loot<Program>(programs);
        foreach(var shell in shells)
        {
            shellLookup.Add(shell.Key, shell);
        }
        foreach(var program in programs)
        {
            programLookup.Add(program.Key, program);
        }
    }

    public bool TryGetProgram(string key, out Program program)
    {
        return programLookup.TryGetValue(key, out program);
    }

    public bool TryGetShell(string key, out Shell shell)
    {
        return shellLookup.TryGetValue(key, out shell);
    }
}
