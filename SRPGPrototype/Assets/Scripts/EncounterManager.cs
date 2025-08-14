﻿using System.Collections;
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
        var encounter = PersistantData.main.mapManager.CurrentEncounter;
        // Setup Money Drop (if applicable)
        if (encounter.giveCompletionMoney)
        {
            GenerateMoneyLoot += encounter.CompletionMoneyData;
        }
        // Initialize Grid
        grid.SetDimensions(encounter.dimensions.x, encounter.dimensions.y);
        grid.CenterAtPosition(BattleGrid.DefaultCenter);
        // Instantiate and add units to the grid
        var units = InitializeUnits(encounter.ambushUnits, encounter.Units);
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
            foreach(var unit in units)
            {
                unit.OnBattleStart(this);
            }
            ui.SetGeneralUIEnabled(true);
            // Start the active encounter
            phaseManager.StartActiveEncounter(EndEncounter);
        }
        unitPlacementUI.Initialize(grid, encounter.spawnPositions, OnUnitPlacementComplete);
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
        SceneTransitionManager.main.TransitionToScene("Cust");
    }

    public void GameOver()
    {
        PersistantData.main.ResetRunData();
        SceneTransitionManager.main.TransitionToScene("StartingShop");
    }

}
