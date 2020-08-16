using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public PhaseManager phaseManager;
    public BattleGrid grid;
    public LootManager loot;
    public LootManager.GenerateProgramLootFn GenerateProgramLoot { get; set; }
    public LootManager.GenerateShellLootFn GenerateShellLoot { get; set; }

    private void Start()
    {
        StartEncounter();
    }

    public void StartEncounter()
    {
        // Get the encounter from the map
        var encounter = PersistantData.main.mapManager.Map.Current.value;
        // Instantiate and add units to the grid
        var units = InitializeUnits(encounter.units);
        // Log the grid's on add event to the phaseManager's add Unit function
        grid.OnAddUnit = phaseManager.AddUnit;
        // Put any like location choosing, etc in here
        // Start the active encounter
        StartCoroutine(phaseManager.StartActiveEncounter(units, EndEncounter));
    }

    private List<Unit> InitializeUnits(IEnumerable<Encounter.UnitEntry> entries)
    {
        var units = new List<Unit>();
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
        StartCoroutine(EndEncounterCr());
    }

    private IEnumerator EndEncounterCr()
    {
        foreach(var unit in grid.FindAll<Unit>())
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
                shellDraws.Add(shellLoot(loot));
        }
        // Generate Program loot
        var progDraws = new LootData<Program>();
        if (GenerateProgramLoot != null)
        {
            foreach (LootManager.GenerateProgramLootFn progLoot in GenerateProgramLoot.GetInvocationList())
                progDraws.Add(progLoot(loot));
        }

        loot.UI.ShowUI(inv, progDraws, shellDraws, EndScene);
    }

    public void EndScene()
    {
        SceneTransitionManager.main.TransitionToScene("Cust");
    }

}
