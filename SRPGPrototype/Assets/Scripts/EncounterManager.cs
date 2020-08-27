using System.Collections;
using System.Collections.Generic;
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
    public LootManager.GenerateProgramLootFn GenerateProgramLoot { get; set; }
    public LootManager.GenerateShellLootFn GenerateShellLoot { get; set; }

    private PlayerUnit player;
    private readonly List<GameObject> spawnPositionObjects = new List<GameObject>();

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
        // Don't allow the encounter to start until the player has been placed
        confirmPlayerPlacementButton.interactable = false;
        confirmPlayerPlacementButton.onClick.AddListener(() => StartEncounter(units));
        // Setup cursor for unit placement
        cursor.OnClick += (pos) => SetPlayerPosition(pos, encounter.spawnPositions);
        cursor.OnCancel += CancelPlayerPlacement;
    }

    private void SetPlayerPosition(Vector2Int pos, List<Vector2Int> spawnPositions)
    {
        if(grid.IsLegalAndEmpty(pos) && (spawnPositions.Count <= 0 || spawnPositions.Contains(pos)))
        {
            if(player == null)
            {
                player = Instantiate(playerPrefab, grid.GetSpace(pos), Quaternion.identity).GetComponent<PlayerUnit>();
                confirmPlayerPlacementButton.interactable = true;
                grid.Add(pos, player);
            }
            else
            {
                grid.MoveAndSetWorldPos(player, pos);
            }
        }
    }

    private void CancelPlayerPlacement()
    {
        if(player != null)
        {
            grid.Remove(player);
            Destroy(player.gameObject);
            player = null;
            confirmPlayerPlacementButton.interactable = false;
        }
    }

    public void StartEncounter(List<Unit> units)
    {
        // Add the player to the list of units
        units.Add(player);
        // Disable the placement UI
        unitPlacementUI.SetActive(false);
        // Clear the cursor's events
        cursor.NullAllActions();
        // Log the grid's on add event to the phaseManager's add Unit function
        grid.OnAddUnit = phaseManager.AddUnit;
        // Destroy the spawn position objects
        spawnPositionObjects.ForEach((obj) => Destroy(obj));
        spawnPositionObjects.Clear();
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
