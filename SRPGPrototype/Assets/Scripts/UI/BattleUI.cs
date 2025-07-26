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
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button linkOutButton;
    [SerializeField] private TextMeshProUGUI linkOutButtonText;
    [SerializeField] private Canvas topBarOverlay;
    [SerializeField] private Canvas generalUI;
    [SerializeField] private InterferenceIconUI[] interferenceIcons;
    [SerializeField] TurnOrderViewerUI turnOrderUI;

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
            turnOrderUI.SetInteractable(value);
        }
    }
    private bool inUnitSelection;

    public bool UnitSelectionUIEnabled
    {
        get => inUnitSelection;
        set
        {
            inUnitSelection = value;
            endTurnButton.interactable = value;
            foreach (var unit in grid.PlayerUnits)
            {
                unit.UI.SetNumberActive(value);
            }
            if (value)
            {
                RefreshLinkOutUI();
            }
            else
            {
                linkOutButton.interactable = false;
            }
        }
    }

    private readonly List<TileUI.Entry> targetPatternEntries = new List<TileUI.Entry>();
    private readonly List<Unit> lastTargets = new List<Unit>();
    private Vector2Int lastSelectedPos;

    private void Awake()
    {
        if (UIManager.main.BattleUI == null)
        {
            UIManager.main.BattleUI = this;
        }
    }

    public void SetUIEnabled(bool value)
    {
        cursor.enabled = value;
        SetGeneralUIEnabled(value);
        TSetopBarOverlayEnabled(value);
        enabled = value;
        menu.enabled = value;
    }

    public void TSetopBarOverlayEnabled(bool value)
    {
        topBarOverlay.enabled = value;
    }

    public void SetGeneralUIEnabled(bool state)
    {
        generalUI.enabled = state;
    }

    public void RefreshLinkOutUI()
    {
        var canLinkout = grid.CanLinkOut(out int numJammers, out int numInterferers, out int threshold);
        if (canLinkout)
        {
            linkOutButton.interactable = true;
            linkOutButtonText.text = "Link Out (L)";
        }
        else
        {
            linkOutButton.interactable = false;
            linkOutButtonText.text = "Link Out Jammed";
        }
        for(int i = 0; i < interferenceIcons.Length; ++i)
        {
            var icon = interferenceIcons[i];
            if(i < numJammers)
            {
                icon.UpdateDisplay(Unit.Jamming.Full);
            }
            else if(i < numJammers + numInterferers)
            {
                icon.UpdateDisplay(Unit.Jamming.Low, (i - numJammers) < threshold);
            }
            else
            {
                icon.UpdateDisplay(Unit.Jamming.None);
            }
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

    public void Initialize()
    {
        UIManager.main.HideAllDescriptionUI();
        turnOrderUI.Initialize(grid, false);
        RefreshLinkOutUI();
        SetGeneralUIEnabled(false);
    }

    private void Update()
    {
        if (generalUI.enabled)
        {
            if(linkOutButton.interactable && Input.GetKeyDown(KeyCode.L))
            {
                linkOutButton.onClick?.Invoke();
            }
            else if(endTurnButton.interactable && Input.GetKeyDown(KeyCode.E))
            {
                endTurnButton.onClick?.Invoke();
            }
        }
        if (!inUnitSelection)
            return;
        for (int i = 0; i < 9; ++i)
        {
            if (Input.GetKeyDown((i + 1).ToString()) && grid.TryGetPlayer(i, out var player))
            {
                SelectPlayer(player.Pos, true);
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if(grid.TryGetPlayer(9, out var player))
            {
                SelectPlayer(player.Pos, true);
                return;
            }
        }
    }

    private void EnterUnitSelection()
    {
        UnitSelectionUIEnabled = true;
        cursor.OnClick = SelectPlayer;
        cursor.OnCancel = null;
        cursor.OnUnHighlight = UIManager.main.HideUnitDescriptionUI;
        cursor.OnHighlight = ShowUnitDescription;
    }

    private void ShowUnitDescription(Vector2Int pos)
    {
        var unit = grid.Get(pos);
        if (unit != null)
        {
            UIManager.main.UnitDescriptionUI.Show(unit);
        }

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
        UIManager.main.UnitDescriptionUI.Hide();
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
        UIManager.main.ActionDescriptionUI.Hide();
        EnterUnitSelection();
    }

    public void EnterActionUI(Action action, Unit unit)
    {
        UIManager.main.ActionDescriptionUI.Hide();
        int currAction = 0;
        lastTargets.Clear();
        lastSelectedPos = Vector2Int.zero;
        action.StartAction(unit);
        var targetRangeEntries = new List<TileUI.Entry>();
        targetRangeEntries = ShowRangePattern(action.SubActions[currAction], unit);
        cursor.OnCancel = () => CancelTargetSelection(action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnClick = (pos) => SelectActionTarget(pos, action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnHighlight = (pos) => HighlightActionTarget(pos, action, unit, ref currAction);
        cursor.OnUnHighlight = (pos) => HideManyTiles(targetPatternEntries);
        var currentPos = cursor.HighlightedPosition;
        if (grid.IsLegal(currentPos))
        {
            HighlightActionTarget(currentPos, action, unit, ref currAction);
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

    private void HighlightActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction)
    {
        var subAction = action.SubActions[currAction];
        if (!GetRangePositions(subAction, unit).Contains(pos))
            return;
        targetPatternEntries.Clear();
        targetPatternEntries.AddRange(ShowTargetPattern(subAction.targetPattern, unit, pos));

    }


    private void SelectActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        var subAction = action.SubActions[currAction];
        if (!GetRangePositions(subAction, unit).Contains(pos))
            return;
        subAction.Use(grid, action, unit, pos, lastTargets, out var targets);
        lastTargets.Clear();
        lastTargets.AddRange(targets);
        lastSelectedPos = pos;
        HideManyTiles(entries);
        cursor.OnUnHighlight?.Invoke(pos);
        if (++currAction >= action.SubActions.Count)
        {
            action.FinishAction(grid, unit);
            if (!playerPhase.CheckEndBattle())
            {
                EnterUnitSelection();
            }
        }
        else
        {
            entries = ShowRangePattern(action.SubActions[currAction], unit);
            var currentPos = cursor.HighlightedPosition;
            if (grid.IsLegal(currentPos))
            {
                HighlightActionTarget(currentPos, action, unit, ref currAction);
            }
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

    private IEnumerable<Vector2Int> GetRangePositions(SubAction subAction, Unit user)
    {
        var rangePos = subAction.OptionFlags.HasFlag(SubAction.Options.RangeBasedOnLastSelectorPos) ? lastSelectedPos : user.Pos;
        return subAction.Range.GetPositions(grid, rangePos, user);
    }

    public List<TileUI.Entry> ShowRangePattern(SubAction sub, Unit user)
    {
        var ret = new List<TileUI.Entry>();
        foreach (var pos in GetRangePositions(sub, user))
        {
            ret.Add(grid.SpawnTileUI(pos, TileUI.Type.RangePattern));
        }
        return ret;
    }

    public void HideManyTiles(IList<TileUI.Entry> entries)
    {
        foreach (var entry in entries)
        {
            grid.RemoveTileUI(entry);
        }
        entries.Clear();
    }

    #endregion
}
