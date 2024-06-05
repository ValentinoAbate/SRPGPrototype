using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public ActionMenu menu;
    public BattleCursor cursor;
    public Button endTurnButton;
    public Button linkOutButton;
    public TextMeshProUGUI linkOutButtonText;
    public TextMeshProUGUI linkOutInfoText;
    public ActionDescriptionUI actionDescription;
    public UnitDescriptionUI unitDescription;

    [Header("Set in Parent Prefab")]
    public BattleGrid grid;
    public PhaseManager phaseManager;
    public PlayerPhase playerPhase;

    public bool PlayerPhaseUIEnabled
    {
        set
        {
            cursor.gameObject.SetActive(value);
            UnitSelectionUIEnabled = value;
        }
    }
    private bool inUnitSelection;

    private bool UnitSelectionUIEnabled
    {
        set
        {
            inUnitSelection = value;
            endTurnButton.interactable = value;
            foreach (var unit in playerPhase.Units)
            {
                unit.unitUI.SetHotKeyActive(value);
            }
            if (value)
            {
                RefreshLinkoutButton();
            }
            else
            {
                linkOutButton.interactable = false;
            }
        }
    }

    public void RefreshLinkoutButton()
    {
        var canLinkout = grid.CanLinkOut(out var interferenceLevel, out int numInterferers);
        if (canLinkout)
        {
            linkOutButton.interactable = true;
            linkOutButtonText.text = "Link Out";
            linkOutInfoText.text = "Link Out Ready";
            return;
        }
        linkOutButton.interactable = false;
        linkOutButtonText.text = "Link Out Blocked";
        if (interferenceLevel == Unit.Interference.Jamming)
        {
            linkOutInfoText.text = $"Link Out Blocked - Defeat all Jamming Units ({numInterferers} left)";
        }
        else
        {
            linkOutInfoText.text = $"Link Out Blocked - Must less than 3 Low Interference Units ({numInterferers} left)";
        }
    }

    public void BeginPlayerTurn()
    {
        EnterUnitSelection();
        PlayerPhaseUIEnabled = true;
    }

    public void EndPlayerTurn()
    {
        menu.Hide();
        PlayerPhaseUIEnabled = false;
    }

    public void OnLinkOutButton()
    {
        phaseManager.EndActiveEncounter();
    }

    public void OnEndTurnButton()
    {
        phaseManager.NextPhase();
    }

    private void Start()
    {
        actionDescription.Hide();
        unitDescription.Hide();
    }

    private void Update()
    {
        if (!inUnitSelection)
            return;
        for (int i = 0; i < playerPhase.Units.Count && i < 9; ++i)
        {
            if (Input.GetKeyDown((i + 1).ToString()) && playerPhase.TryGetPlayer(i, out var player))
            {
                SelectPlayer(player.Pos, true);
                return;
            }
        }
        if (playerPhase.Units.Count >= 10 && Input.GetKeyDown(KeyCode.Alpha0))
        {
            if(playerPhase.TryGetPlayer(9, out var player))
            {
                SelectPlayer(player.Pos, true);
                return;
            }
        }
    }

    private void EnterUnitSelection()
    {
        playerPhase.CheckEndPhase();
        UnitSelectionUIEnabled = true;
        cursor.OnClick = SelectPlayer;
        cursor.OnCancel = null;
        cursor.OnUnHighlight = HideUnitDescription;
        cursor.OnHighlight = ShowUnitDescription;
    }

    private void ShowUnitDescription(Vector2Int pos)
    {
        var unit = grid.Get(pos);
        if (unit != null)
            unitDescription.Show(unit);
    }

    private void HideUnitDescription(Vector2Int pos)
    {
        unitDescription.Hide();
    }

    private void SelectPlayer(Vector2Int pos)
    {
        SelectPlayer(pos, false);
    }

    private void SelectPlayer(Vector2Int pos, bool fromHotKey)
    {
        var playerUnit = grid.Get<PlayerUnit>(pos);
        if (playerUnit != null)
        {
            EnterActionMenu(playerUnit, fromHotKey);
        }
    }

    private void EnterActionMenu(Unit unit, bool fromHotKey)
    {
        unitDescription.Hide();
        UnitSelectionUIEnabled = false;
        menu.Show(grid, this, unit, fromHotKey);
        cursor.OnClick = null;
        cursor.OnCancel = CancelActionMenu;
        cursor.OnUnHighlight = null;
        cursor.OnHighlight = null;
    }

    private void CancelActionMenu()
    {
        menu.Hide();
        actionDescription.Hide();
        EnterUnitSelection();
    }

    public void EnterActionUI(Action action, Unit unit)
    {
        actionDescription.Hide();
        int currAction = 0;
        action.StartAction(unit);
        var targetRangeEntries = new List<TileUI.Entry>();
        var targetPatternEntires = new List<TileUI.Entry>();
        targetRangeEntries = ShowRangePattern(action.SubActions[currAction].Range, unit);
        cursor.OnCancel = () => CancelTargetSelection(action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnClick = (pos) => SelectActionTarget(pos, action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnHighlight = (pos) => HighlightActionTarget(pos, action, unit, ref currAction, ref targetPatternEntires);
        cursor.OnUnHighlight = (pos) => HideManyTiles(targetPatternEntires);
        var currentPos = cursor.HighlightedPosition;
        if (grid.IsLegal(currentPos))
        {
            HighlightActionTarget(currentPos, action, unit, ref currAction, ref targetPatternEntires);
        }
    }

    private void ExitActionUI(Unit unit, ref List<TileUI.Entry> entries)
    {
        if(entries != null)
        {
            HideManyTiles(entries);
            cursor.OnUnHighlight?.Invoke(unit.Pos);
            entries.Clear();
        }
        EnterActionMenu(unit, false);
    }

    private void HighlightActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        var subAction = action.SubActions[currAction];
        if (!subAction.Range.GetPositions(grid, unit.Pos).Contains(pos))
            return;
        entries = ShowTargetPattern(subAction.targetPattern, unit, pos);

    }


    private void SelectActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        var subAction = action.SubActions[currAction];
        if (!subAction.Range.GetPositions(grid, unit.Pos).Contains(pos))
            return;
        subAction.Use(grid, action, unit, pos);
        HideManyTiles(entries);
        cursor.OnUnHighlight?.Invoke(pos);
        if (++currAction >= action.SubActions.Count)
        {
            action.FinishAction(unit);
            EnterUnitSelection();
        }
        else
        {
            entries = ShowRangePattern(action.SubActions[currAction].Range, unit);
        }
    }

    private void CancelTargetSelection(Action action, Unit unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        if (currAction <= 0)
        {
            ExitActionUI(unit, ref entries);
        }
        else
        {
            //HideManyTiles(entries);
            //var subAction = action.subActions[--currAction];
            //entries = ShowPattern(subAction.range, unit.Pos, TileUI.Type.CustGreen);
        }
    }

    #region Tile UI Display

    public List<TileUI.Entry> ShowPattern(Pattern p, Vector2Int center, TileUI.Type type)
    {
        var ret = new List<TileUI.Entry>();
        foreach (var pos in p.OffsetsShifted(center))
        {
            ret.Add(grid.SpawnTileUI(pos, type));
        }
        return ret;
    }

    public List<TileUI.Entry> ShowTargetPattern(TargetPattern p, Unit user, Vector2Int target)
    {
        var ret = new List<TileUI.Entry>();
        foreach (var pos in p.Target(grid, user, target))
        {
            ret.Add(grid.SpawnTileUI(pos, TileUI.Type.TargetPattern));
        }
        return ret;
    }

    public List<TileUI.Entry> ShowRangePattern(RangePattern p, Unit user)
    {
        var ret = new List<TileUI.Entry>();
        foreach (var pos in p.GetPositions(grid, user.Pos))
        {
            ret.Add(grid.SpawnTileUI(pos, TileUI.Type.RangePattern));
        }
        return ret;
    }

    public void HideManyTiles(IEnumerable<TileUI.Entry> entries)
    {
        foreach (var entry in entries)
        {
            grid.RemoveTileUI(entry);
        }
    }

    #endregion
}
