using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public BattleGrid grid;
    public PlayerPhase playerPhase;
    public ActionMenu menu;
    public BattleCursor cursor;
    public Button endTurnButton;
    public ActionDescriptionUI actionDescription;

    public bool PlayerPhaseUIEnabled
    {
        set
        {
            cursor.gameObject.SetActive(value);
            endTurnButton.interactable = value;
        }
    }


    private void Start()
    {
        actionDescription.Hide();
        EnterUnitSelection();
    }

    private void EnterUnitSelection()
    {
        playerPhase.CheckEndPhase();
        cursor.OnClick = SelectPlayer;
        cursor.OnCancel = null;
        cursor.OnUnHighlight = null;
        cursor.OnHighlight = null;
    }

    private void SelectPlayer(Vector2Int pos)
    {
        var playerUnit = grid.Get<PlayerUnit>(pos);
        if (playerUnit != null)
        {
            EnterActionMenu(playerUnit);
        }
    }

    private void EnterActionMenu(Unit unit)
    {
        menu.Show(this, unit);
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
        targetRangeEntries = ShowPattern(action.subActions[currAction].range, unit.Pos, TileUI.Type.CustGreen);
        cursor.OnCancel = () => CancelTargetSelection(action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnClick = (pos) => SelectActionTarget(pos, action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnHighlight = (pos) => HighlightActionTarget(pos, action, unit, ref currAction, ref targetPatternEntires);
        cursor.OnUnHighlight = (pos) => HideManyTiles(targetPatternEntires);
    }

    private void ExitActionUI(Unit unit, ref List<TileUI.Entry> entries)
    {
        if(entries != null)
        {
            HideManyTiles(entries);
            cursor.OnUnHighlight?.Invoke(unit.Pos);
            entries.Clear();
        }
        EnterActionMenu(unit);
    }

    private void HighlightActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        var subAction = action.subActions[currAction];
        if (!subAction.range.OffsetsShifted(unit.Pos - subAction.range.Center).Contains(pos))
            return;
        entries = ShowTargetPattern(subAction.targetPattern, unit, pos, TileUI.Type.CustWhite);

    }


    private void SelectActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        var subAction = action.subActions[currAction];
        if (!subAction.range.OffsetsShifted(unit.Pos - subAction.range.Center).Contains(pos))
            return;
        subAction.Use(grid, action, unit, pos);
        HideManyTiles(entries);
        cursor.OnUnHighlight?.Invoke(pos);
        if (++currAction >= action.subActions.Count)
        {
            action.FinishAction(unit);
            EnterUnitSelection();
        }
        else
        {
            entries = ShowPattern(action.subActions[currAction].range, unit.Pos, TileUI.Type.CustGreen);
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
            HideManyTiles(entries);
            var subAction = action.subActions[--currAction];
            entries = ShowPattern(subAction.range, unit.Pos, TileUI.Type.CustGreen);
        }
    }

    #region Tile UI Display

    public List<TileUI.Entry> ShowPattern(Pattern p, Vector2Int center, TileUI.Type type)
    {
        var ret = new List<TileUI.Entry>();
        foreach (var pos in p.OffsetsShifted(center - p.Center))
        {
            ret.Add(grid.SpawnTileUI(pos, type));
        }
        return ret;
    }

    public List<TileUI.Entry> ShowTargetPattern(TargetPattern p, Unit user, Vector2Int target, TileUI.Type type)
    {
        var ret = new List<TileUI.Entry>();
        foreach (var pos in p.Target(grid, user, target))
        {
            ret.Add(grid.SpawnTileUI(pos, type));
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
