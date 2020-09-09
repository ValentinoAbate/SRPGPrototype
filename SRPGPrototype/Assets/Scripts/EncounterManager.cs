using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
    public PhaseManager phaseManager;
    public BattleGrid grid;
    public LootManager loot;
    public BattleCursor cursor;
    public GameObject playerPrefab;
    public GameObject unitPlacementUI;
    public Button confirmPlayerPlacementButton;
    public GameObject spawnPositionPrefab;
    public Transform outOfBoundsPos;
    public LootManager.GenerateProgramLootFn GenerateProgramLoot { get; set; }
    public LootManager.GenerateShellLootFn GenerateShellLoot { get; set; }

    private readonly List<GameObject> spawnPositionObjects = new List<GameObject>();
    private readonly Stack<PlayerUnit> spawnedUnits = new Stack<PlayerUnit>();
    private readonly Stack<PlayerUnit> unitsToSpawn = new Stack<PlayerUnit>();

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Get the encounter from the map
        var encounter = PersistantData.main.mapManager.Map.Current.value;
        // Instantiate and add units to the grid
        var units = InitializeUnits(encounter.units);
        // Instantiate spawn position objects
        foreach(var pos in encounter.spawnPositions)
        {
            var spawnPosObj = Instantiate(spawnPositionPrefab);
            spawnPosObj.transform.position = grid.GetSpace(pos);
            spawnPositionObjects.Add(spawnPosObj);
        }

        // Add player units to spawn stack
        unitsToSpawn.Clear();
        spawnedUnits.Clear();
        // Add drones (if applicable) to the spawn stack
        foreach (var droneShell in PersistantData.main.inventory.DroneShells)
        {
            var droneUnit = Instantiate(droneShell.SoulCoreUnitPrefab, outOfBoundsPos.position, Quaternion.identity).GetComponent<PlayerDroneUnit>();
            droneUnit.SetShell(droneShell);
            unitsToSpawn.Push(droneUnit);
        }
        // Add the player to the spawn stack
        unitsToSpawn.Push(Instantiate(playerPrefab, outOfBoundsPos.position, Quaternion.identity).GetComponent<PlayerUnit>());

        // Don't allow the encounter to start until the player has been placed
        confirmPlayerPlacementButton.interactable = false;
        confirmPlayerPlacementButton.onClick.AddListener(() => StartEncounter(units));
        // Setup cursor for unit placement
        cursor.OnClick += (pos) => PlaceUnit(pos, encounter.spawnPositions);
        cursor.OnCancel += CancelUnitPlacement;
    }

    private void PlaceUnit(Vector2Int pos, List<Vector2Int> spawnPositions)
    {
        if (grid.IsLegalAndEmpty(pos) && (spawnPositions.Count <= 0 || spawnPositions.Contains(pos)))
        {
            if(unitsToSpawn.Count >= 1)
            {
                var unit = unitsToSpawn.Pop();
                grid.Add(pos, unit);
                unit.transform.position = grid.GetSpace(pos);
                spawnedUnits.Push(unit);
                if (unitsToSpawn.Count <= 0)
                {
                    confirmPlayerPlacementButton.interactable = true;
                }
            }
            else
            {
                grid.MoveAndSetWorldPos(spawnedUnits.Peek(), pos);
            }
        }
    }

    private void CancelUnitPlacement()
    {
        if(spawnedUnits.Count >= 1)
        {
            var unit = spawnedUnits.Pop();
            unit.transform.position = outOfBoundsPos.position;
            grid.Remove(unit);
            unitsToSpawn.Push(unit);
            confirmPlayerPlacementButton.interactable = false;
        }
    }

    public void StartEncounter(List<Unit> units)
    {
        // Add the player units to the list of units
        units.AddRange(spawnedUnits);
        // Disable the placement UI
        unitPlacementUI.SetActive(false);
        // Clear the cursor's events
        cursor.NullAllActions();
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
