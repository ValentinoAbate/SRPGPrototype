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
    [SerializeField] private UnitPlacementUI unitPlacementUI;
    [SerializeField] private Button confirmPlayerPlacementButton;
    [SerializeField] private GameObject spawnPositionPrefab;
    [SerializeField] private BattleUI ui;
    public LootData<Program>.GenerateLootFunction GenerateProgramLoot { get; set; }
    public LootData<Shell>.GenerateLootFunction GenerateShellLoot { get; set; }
    public System.Func<LootUI.MoneyData> GenerateMoneyLoot { get; set; }

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
        grid.CenterAtPosition(BattleGrid.DefaultCenter);
        // Instantiate and add units to the grid
        var units = InitializeUnits(encounter.units);
        // Instantiate spawn position objects
        var spawnPositionObjects = new List<GameObject>(encounter.spawnPositions.Count);
        foreach(var pos in encounter.spawnPositions)
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
            units.ForEach((u) => StartCoroutine(u.OnBattleStart(this)));
            ui.SetGeneralUIEnabled(true);
            // Start the active encounter
            phaseManager.StartActiveEncounter(EndEncounter);
        }
        unitPlacementUI.Initialize(grid, encounter.spawnPositions, OnUnitPlacementComplete);
    }

    private List<Unit> InitializeUnits(IReadOnlyCollection<Encounter.UnitEntry> entries)
    {
        var units = new List<Unit>(entries.Count + PersistantData.main.inventory.DroneShells.Length + 1);
        foreach (var entry in entries)
        {
            var unit = Instantiate(entry.unit).GetComponent<Unit>();
            grid.Add(entry.pos, unit);
            unit.transform.position = grid.GetSpace(unit.Pos);
            units.Add(unit);
        }
        return units;
    }

    public void EndEncounter()
    {
        ui.DisableInput();
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
        moneyData.Add(new LootUI.MoneyData(RandomUtils.RandomU.instance.RandomInt(6, 13)));

        loot.UI.ShowUI(inv, progDraws, shellDraws, moneyData, EndScene);
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
