using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager main;
    [SerializeField] private PhaseManager phaseManager;
    public BattleGrid Grid => grid;
    [SerializeField] private BattleGrid grid;
    [SerializeField] private UnitPlacementUI unitPlacementUI;
    [SerializeField] private Button confirmPlayerPlacementButton;
    [SerializeField] private GameObject spawnPositionPrefab;
    [SerializeField] private BattleUI ui;
    public LootData<Program>.GenerateLootFunction GenerateProgramLoot { get; set; }
    public LootData<Shell>.GenerateLootFunction GenerateShellLoot { get; set; }
    public System.Func<LootUI.MoneyData> GenerateMoneyLoot { get; set; }

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Get the encounter from the map
        var encounter = PersistantData.main.CurrentEncounter;
        // Setup Money Drop (if applicable)
        if (encounter.giveCompletionMoney)
        {
            GenerateMoneyLoot += encounter.CompletionMoneyData;
        }
        // Initialize Grid
        grid.SetDimensions(encounter.dimensions.x, encounter.dimensions.y);
        grid.CenterAtPosition(BattleGrid.DefaultCenter);

        // Load battle, else initialize from next encounters
        if (PersistantData.main.BattleData.LoadedUnits.Count > 0)
        {
            // Load units
            foreach(var unit in PersistantData.main.BattleData.LoadedUnits)
            {
                unit.transform.SetParent(transform);
                Grid.Add(unit.Pos, unit);
                unit.transform.position = Grid.GetSpace(unit.Pos);
            }
            PersistantData.main.BattleData.LoadedUnits.Clear();

            // Initialize Battle UI
            ui.Initialize();
            ui.SetGeneralUIEnabled(true);
            // Hide unit placement UI
            unitPlacementUI.Hide();
            // Start the active encounter
            phaseManager.StartActiveEncounter(EndEncounter, false);
            ui.BeginPlayerTurn();
        }
        else
        {
            // Instantiate and add units to the grid
            var units = InitializeUnits(encounter.ambushUnits, encounter.Units);
            // Instantiate spawn position objects
            var spawnPositionObjects = new List<GameObject>(encounter.spawnPositions.Count);
            foreach (var pos in encounter.spawnPositions)
            {
                var spawnPosObj = Instantiate(spawnPositionPrefab);
                spawnPosObj.transform.position = grid.GetSpace(pos);
                spawnPositionObjects.Add(spawnPosObj);
            }
            // Initialize Battle UI
            ui.Initialize();
            void OnUnitPlacementComplete(IEnumerable<Unit> spawnedUnits)
            {
                spawnPositionObjects.ForEach(Destroy);
                spawnPositionObjects.Clear();
                units.AddRange(spawnedUnits);
                units.Sort();
                foreach (var unit in units)
                {
                    unit.OnBattleStart(this);
                }
                ui.SetGeneralUIEnabled(true);
                // Start the active encounter
                phaseManager.StartActiveEncounter(EndEncounter, true);
            }
            unitPlacementUI.Initialize(grid, encounter.spawnPositions, OnUnitPlacementComplete);
            SaveManager.Save(SaveManager.State.UnitPlacement);
        }
    }

    private List<Unit> InitializeUnits(IReadOnlyCollection<Encounter.UnitEntry> ambushUnitEntries, IReadOnlyCollection<Encounter.UnitEntry> entries)
    {
        var units = new List<Unit>(entries.Count + ambushUnitEntries.Count + PersistantData.main.inventory.DroneShells.Count() + 1);
        foreach (var entry in ambushUnitEntries)
        {
            if (TrySpawnUnit(entry, out var unit))
                units.Add(unit);
        }
        foreach (var entry in entries)
        {
            if (TrySpawnUnit(entry, out var unit))
                units.Add(unit);
        }
        return units;
    }

    private bool TrySpawnUnit(Encounter.UnitEntry entry, out Unit unit)
    {
        if (!grid.IsLegalAndEmpty(entry.pos))
        {
            unit = null;
            return false;
        }
        unit = Instantiate(entry.unit).GetComponent<Unit>();
        grid.Add(entry.pos, unit);
        unit.transform.position = grid.GetSpace(unit.Pos);
        return true;
    }

    public void EndEncounter()
    {
        ui.SetUIEnabled(false);
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
        {
            var battleEndCr = unit.OnBattleEnd(this);
            if(battleEndCr != null)
            {
                yield return battleEndCr;
            }
        }
        GenerateAndShowLoot();
    }

    private void GenerateAndShowLoot()
    {
        var inv = PersistantData.main.inventory;
        var loot = PersistantData.main.loot;

        // Generate Shell Loot
        var shellDraws = new LootData<Shell>();
        if (GenerateShellLoot != null)
        {
            foreach (LootData<Shell>.GenerateLootFunction shellLootGenerator in GenerateShellLoot.GetInvocationList())
            {
                shellDraws.Add(loot.ShellLoot, shellLootGenerator);
            }

        }
        // Generate Program loot
        var progDraws = new LootData<Program>();
        if (GenerateProgramLoot != null)
        {
            foreach (LootData<Program>.GenerateLootFunction progLootGenerator in GenerateProgramLoot.GetInvocationList())
            {
                progDraws.Add(loot.ProgramLoot, progLootGenerator);
            }
        }

        var moneyData = new List<LootUI.MoneyData>();
        if(GenerateMoneyLoot != null)
        {
            foreach(System.Func<LootUI.MoneyData> moneyLootGenerator in GenerateMoneyLoot.GetInvocationList())
            {
                moneyData.Add(moneyLootGenerator());
            }
        }

        loot.UI.ShowUI(inv, progDraws, shellDraws, moneyData, EndScene);
    }

    public void EndScene()
    {
        PersistantData.main.mapManager.GenerateNextEncounters();
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.CustSceneName);
    }

    public void GameOver()
    {
        SaveManager.ClearRunData();
        PersistantData.main.ResetRunData();
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.TitleScene);
    }

    public SaveManager.BattleEncounterData Save()
    {
        var data = new SaveManager.BattleEncounterData()
        {
            data = new List<SaveManager.UnitData>()
        };
        foreach (var unit in Grid)
        {
            data.data.Add(unit.Save());
        }
        return data;
    }
}
