using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private PhaseManager phaseManager;
    public BattleGrid Grid => grid;
    [SerializeField] private BattleGrid grid;
    [SerializeField] private LootManager loot;
    [SerializeField] private UnitPlacementUI unitPlacementUI;
    [SerializeField] private Button confirmPlayerPlacementButton;
    [SerializeField] private GameObject spawnPositionPrefab;
    [SerializeField] private Transform battleGridCenter;
    public LootManager.GenerateProgramLootFn GenerateProgramLoot { get; set; }
    public LootManager.GenerateShellLootFn GenerateShellLoot { get; set; }

    private readonly List<GameObject> spawnPositionObjects = new List<GameObject>();
    private readonly List<Unit> units = new List<Unit>();

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Get the encounter from the map
        var encounter = PersistantData.main.mapManager.Map.Current.value;
        // Initialize Grid
        grid.SetDimensions(encounter.dimensions.x, encounter.dimensions.y);
        grid.CenterAtPosition(battleGridCenter.position);
        // Instantiate and add units to the grid
        InitializeUnits(encounter.units);
        // Instantiate spawn position objects
        spawnPositionObjects.Clear();
        spawnPositionObjects.EnsureCapacity(encounter.spawnPositions.Count);
        foreach(var pos in encounter.spawnPositions)
        {
            var spawnPosObj = Instantiate(spawnPositionPrefab);
            spawnPosObj.transform.position = grid.GetSpace(pos);
            spawnPositionObjects.Add(spawnPosObj);
        }
        unitPlacementUI.Initialize(grid, encounter.spawnPositions, StartEncounter);
    }

    private void StartEncounter(IEnumerable<Unit> spawnedUnits)
    {
        // Add the player units to the list of units
        units.AddRange(spawnedUnits);

        // Log the grid's on add event to the phaseManager's add Unit function
        grid.OnAddUnit = phaseManager.AddUnit;
        // Destroy the spawn position objects
        spawnPositionObjects.ForEach((obj) => Destroy(obj));
        spawnPositionObjects.Clear();
        // Apply the units' on start abilities
        units.ForEach((u) => StartCoroutine(u.OnBattleStart(this)));
        // Start the active encounter
        StartCoroutine(phaseManager.StartActiveEncounter(units, EndEncounter));
    }

    private void InitializeUnits(IReadOnlyCollection<Encounter.UnitEntry> entries)
    {
        units.Clear();
        units.EnsureCapacity(entries.Count + PersistantData.main.inventory.DroneShells.Length + 1);
        foreach (var entry in entries)
        {
            var unit = Instantiate(entry.unit).GetComponent<Unit>();
            grid.Add(entry.pos, unit);
            unit.transform.position = grid.GetSpace(unit.Pos);
            units.Add(unit);
        }
    }

    public void EndEncounter()
    {
        if (grid.MainPlayerDead)
        {
            GameOver();
            return;
        }
        StartCoroutine(EndEncounterCr());
    }

    private IEnumerator EndEncounterCr()
    {
        foreach(var unit in grid)
            yield return StartCoroutine(unit.OnBattleEnd(this));
        GenerateAndShowLoot();
    }

    private void GenerateAndShowLoot()
    {
        var inv = PersistantData.main.inventory;

        // Generate Shell Loot
        var shellDraws = new LootData<Shell>();
        if (GenerateShellLoot != null)
        {
            foreach (LootManager.GenerateShellLootFn shellLoot in GenerateShellLoot.GetInvocationList())
                shellDraws.Add(shellLoot(loot.ShellLoot));
        }
        // Generate Program loot
        var progDraws = new LootData<Program>();
        if (GenerateProgramLoot != null)
        {
            foreach (LootManager.GenerateProgramLootFn progLoot in GenerateProgramLoot.GetInvocationList())
                progDraws.Add(progLoot(loot.ProgramLoot));
        }

        loot.UI.ShowUI(inv, progDraws, shellDraws, EndScene);
    }

    public void EndScene()
    {
        SceneTransitionManager.main.TransitionToScene("Cust");
    }

    public void GameOver()
    {
        PersistantData.main.ResetRunData();
        SceneTransitionManager.main.TransitionToScene("StartingShop");
    }

}
