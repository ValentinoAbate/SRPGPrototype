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
        EnterUnitSelection();
    }

    private void EnterUnitSelection()
    {
        playerPhase.CheckEndPhase();
        cursor.OnClick = SelectPlayer;
        cursor.OnCancel = null;
    }

    private void SelectPlayer(Vector2Int pos)
    {
        var playerUnit = grid.Get<UnitPlayer>(pos);
        if (playerUnit != null)
        {
            EnterActionMenu(playerUnit);
        }
    }

    private void EnterActionMenu(Combatant unit)
    {
        menu.Show(this, unit);
        cursor.OnClick = null;
        cursor.OnCancel = CancelActionMenu;
    }

    private void CancelActionMenu()
    {
        menu.Hide();
        EnterUnitSelection();
    }

    public void EnterActionUI(Action action, Combatant unit)
    {
        int currAction = 0;
        var entries = new List<TileUI.Entry>();
        cursor.OnCancel = () => CancelTargetSelection(action, unit, ref currAction, ref entries);
        entries = ShowPattern(action.subActions[currAction].range, unit.Pos, TileUI.Type.CustGreen);
        cursor.OnClick = (pos) => SelectActionTarget(pos, action, unit, ref currAction, ref entries);
    }

    private void ExitActionUI(Combatant unit, ref List<TileUI.Entry> entries)
    {
        if(entries != null)
        {
            HideManyTiles(entries);
            entries.Clear();
        }
        EnterActionMenu(unit);
    }

    private void SelectActionTarget(Vector2Int pos, Action action, Combatant unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        var subAction = action.subActions[currAction];
        if (!subAction.range.OffsetsShifted(unit.Pos - subAction.range.Center).Contains(pos))
            return;
        subAction.Use(grid, unit, pos);
        HideManyTiles(entries);
        if (++currAction >= action.subActions.Count)
        {
            unit.AP -= action.APCost;
            EnterUnitSelection();
        }
        else
        {
            entries = ShowPattern(action.subActions[currAction].range, unit.Pos, TileUI.Type.CustGreen);
        }
    }

    private void CancelTargetSelection(Action action, Combatant unit, ref int currAction, ref List<TileUI.Entry> entries)
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

    public List<TileUI.Entry> ShowTargetPattern(TargetPattern p, Combatant user, Vector2Int target, TileUI.Type type)
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
