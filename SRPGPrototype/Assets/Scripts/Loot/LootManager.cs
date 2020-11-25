using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public delegate List<Shell> GenerateShellLootFn(LootManager manager);
    public delegate List<Program> GenerateProgramLootFn(LootManager manager);

    public List<Program> programs = new List<Program>();
    public List<Shell> shells = new List<Shell>();

    public Loot<Shell> ShellLoot { get; private set; }
    public Loot<Program> ProgramLoot { get; private set; }
    public LootUI UI => lootUI;
    [SerializeField] private LootUI lootUI = null;

    private void Awake()
    {
        ShellLoot = new Loot<Shell>(shells);
        ProgramLoot = new Loot<Program>(programs);
    }
}
