using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitPlacementUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI placementText;
    [SerializeField] private TextMeshProUGUI unitsLeftText;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform outOfBoundsPos;
    [Header("Set In Parent Prefab")]
    [SerializeField] private BattleCursor cursor;

    private BattleGrid grid;
    private List<Vector2Int> spawnPositions;
    private readonly Stack<Unit> spawnedUnits = new Stack<Unit>();
    private readonly Stack<Unit> unitsToSpawn = new Stack<Unit>();

    private System.Action<IEnumerable<Unit>> onComplete;

    public void Initialize(BattleGrid grid, List<Vector2Int> spawnPositions, System.Action<IEnumerable<Unit>> onComplete)
    {
        this.spawnPositions = spawnPositions;
        this.grid = grid;
        this.onComplete = onComplete;
        // Add player units to spawn stack
        unitsToSpawn.Clear();
        spawnedUnits.Clear();
        // Add drones (if applicable) to the spawn stack
        var droneShells = new List<Shell>(PersistantData.main.inventory.DroneShells);
        for (int i = droneShells.Count - 1; i >= 0; --i)
        {
            var droneShell = droneShells[i];
            var droneUnit = Instantiate(droneShell.SoulCoreUnitPrefab, outOfBoundsPos.position, Quaternion.identity).GetComponent<PlayerDroneUnit>();
            droneUnit.SetShell(droneShell);
            droneUnit.HotkeyIndex = i + 1;
            unitsToSpawn.Push(droneUnit);
        }
        // Add the player to the spawn stack
        var player = Instantiate(playerPrefab, outOfBoundsPos.position, Quaternion.identity).GetComponent<PlayerUnit>();
        player.HotkeyIndex = 0;
        player.IsMain = true;
        unitsToSpawn.Push(player);

        UpdateText();

        // Don't allow the encounter to start until the player has been placed
        confirmButton.interactable = false;
        canvas.enabled = true;
        enabled = true;

        // Setup Cursor
        cursor.OnClick += PlaceUnit;
        cursor.OnCancel += CancelUnitPlacement;
        cursor.OnHighlight = ShowUnitDescription;
        cursor.OnUnHighlight = UIManager.main.HideUnitDescriptionUI;
    }

    // Enable unit descriptions
    private void ShowUnitDescription(Vector2Int pos)
    {
        var unit = grid.Get(pos);
        if (unit != null)
        {
            UIManager.main.UnitDescriptionUI.Show(unit);
        }
    }
    public void FinishUnitPlacement()
    {
        if(unitsToSpawn.Count <= 0)
        {
            Complete();
            return;
        }
        cursor.enabled = false;
        enabled = false;
        PopupManager.main.ShowConfirmationPopup(CompleteOnConfirmation, descriptionText: GetConfirmationDesc());
    }

    private string GetConfirmationDesc()
    {
        if(unitsToSpawn.Count == 1)
        {
            return "You still have a unit left to place.\nWould you like deploy anyway?";
        }
        return $"You still have {unitsToSpawn.Count} units left to place.\nWould you like deploy anyway?";
    }

    private void CompleteOnConfirmation(bool success)
    {
        if (success)
        {
            Complete();
        }
        else
        {
            cursor.enabled = true;
            enabled = true;
        }
    }

    private void Complete()
    {
        // Clear the cursor's events
        cursor.NullAllActions();
        canvas.enabled = false;
        enabled = false;
        onComplete?.Invoke(spawnedUnits);
    }

    public void PlaceUnit(Vector2Int pos)
    {
        if (grid.IsLegalAndEmpty(pos) && (spawnPositions.Count <= 0 || spawnPositions.Contains(pos)))
        {
            if (unitsToSpawn.Count >= 1)
            {
                var unit = unitsToSpawn.Pop();
                grid.Add(pos, unit);
                unit.transform.position = grid.GetSpace(pos);
                spawnedUnits.Push(unit);
                confirmButton.interactable = true;
                UpdateText();
            }
            else
            {
                grid.MoveAndSetWorldPos(spawnedUnits.Peek(), pos);
            }
        }
    }

    private void CancelUnitPlacement()
    {
        if (spawnedUnits.Count >= 1)
        {
            var unit = spawnedUnits.Pop();
            unit.transform.position = outOfBoundsPos.position;
            grid.Remove(unit);
            unitsToSpawn.Push(unit);
            confirmButton.interactable = spawnedUnits.Count > 0;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if(unitsToSpawn.Count > 0)
        {
            placementText.text = $"Now Placing: {unitsToSpawn.Peek().DisplayName}";
            unitsLeftText.text = unitsToSpawn.Count == 1 ? "1 Unit Left": $"{unitsToSpawn.Count} Units Left";
        }
        else
        {
            placementText.text = "All Units Placed";
            unitsLeftText.text = "0 Units Left";
        }
    }

    private void Update()
    {
        if(confirmButton.interactable && Input.GetKeyDown(KeyCode.C))
        {
            FinishUnitPlacement();
        }
    }
}
