using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public delegate List<Shell> GenerateShellLootFn(LootManager manager);
    public delegate List<Program> GenerateProgramLootFn(LootManager manager);

    public List<Program> programs = new List<Program>();
    public List<Shell> shells = new List<Shell>();

    public Loot<Shell> ShellLoot => shellLoot;
    private Loot<Shell> shellLoot;
    public Loot<Program> ProgramLoot => programLoot;
    private Loot<Program> programLoot;
    public LootUI UI => lootUI;
    [SerializeField] private LootUI lootUI = null;

    private void Awake()
    {
        shellLoot = new Loot<Shell>(shells);
        programLoot = new Loot<Program>(programs);
    }
}
