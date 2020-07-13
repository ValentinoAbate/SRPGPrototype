using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleUI : MonoBehaviour
{
    public BattleGrid grid;
    public ActionMenu menu;
    public BattleCursor cursor;

    private void Start()
    {
        EnterUnitSelection();
    }

    private void EnterUnitSelection()
    {
        cursor.OnClick = SelectPlayer;
        cursor.OnCancel = null;
    }

    private void SelectPlayer(Vector2Int pos)
    {
        var playerUnit = grid.Get<UnitPlayer>(pos);
        if (playerUnit != null)
        {
            menu.Show(this, playerUnit);
            cursor.OnClick = null;
            cursor.OnCancel = CancelMenu;
        }
    }

    private int currAction = 0;
    private List<TileUI.Entry> entries;
    public void ActionUI(Action action, Combatant unit)
    {
        currAction = 0;
        entries = new List<TileUI.Entry>();
        cursor.OnCancel = () =>
        {
            if(currAction <= 0)
            {
                menu.Show(this, unit);
                cursor.OnClick = null;
                HideManyTiles(entries);
                entries.Clear();
                cursor.OnCancel = CancelMenu;
            }
            else
            {
                HideManyTiles(entries);
                entries = ShowPattern(action.subActions[--currAction].range, unit.Pos, TileUI.Type.CustGreen);
            }
        };
        entries = ShowPattern(action.subActions[currAction].range, unit.Pos, TileUI.Type.CustGreen);
        cursor.OnClick = (pos) =>
        {
            var subAction = action.subActions[currAction];
            if (!subAction.range.OffsetsShifted(unit.Pos - subAction.range.Center).Contains(pos))
                return;
            subAction.Use(grid, unit, pos);
            HideManyTiles(entries);
            if(++currAction >= action.subActions.Count)
            {
                cursor.OnCancel = null;
                EnterUnitSelection();
            }
            else
            {
                entries = ShowPattern(action.subActions[currAction].range, unit.Pos, TileUI.Type.CustGreen);
            }
        };
    }

    private void CancelMenu()
    {
        menu.Hide();
        EnterUnitSelection();
        cursor.OnCancel = null;
    }

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
}
