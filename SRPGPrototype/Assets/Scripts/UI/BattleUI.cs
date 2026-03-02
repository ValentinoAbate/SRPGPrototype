using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public const int MaxHotkeyIndex = 9;
    public ActionMenu menu;
    public BattleCursor cursor;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button linkOutButton;
    [SerializeField] private TextMeshProUGUI linkOutButtonText;
    [SerializeField] private Canvas topBarOverlay;
    [SerializeField] private Canvas generalUI;
    [SerializeField] private InterferenceIconUI[] interferenceIcons;

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
            UIManager.main.TurnOrderUI.SetInteractable(value);
            UIManager.main.UnitUIViewer.SetInteractable(value);
            UIManager.main.UnitUIViewer.HideUI();
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
            foreach (var unit in grid)
            {
                if (unit.UnitTeam != Unit.Team.Player)
                    continue;
                if(unit.HotkeyIndex >= 0)
                {
                    unit.UI.SetNumberText((unit.HotkeyIndex + 1).ToString());
                    unit.UI.SetNumberActive(value);
                }
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
        SetTopBarOverlayEnabled(value);
        enabled = value;
        menu.enabled = value;
    }

    public void SetTopBarOverlayEnabled(bool value)
    {
        topBarOverlay.enabled = value;
        if (value)
        {
            UIManager.main.TurnOrderUI.Show();
            UIManager.main.UnitUIViewer.Show();
        }
        else
        {
            UIManager.main.TurnOrderUI.Hide();
            UIManager.main.UnitUIViewer.Hide();
        }
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
        if(grid.CanLinkOut(out _, out _, out _))
        {
            PlayerPhaseUIEnabled = false;
            PopupManager.main.ShowConfirmationPopup(OnEndTurnConfirmed, "Are you sure?", $"You are able to link out.", "End Turn");
        }
        else
        {
            phaseManager.NextPhase();
        }
    }

    private void OnEndTurnConfirmed(bool confirmed)
    {
        if (confirmed)
        {
            phaseManager.NextPhase();
        }
        else
        {
            PlayerPhaseUIEnabled = true;
        }
    }

    public void Initialize()
    {
        UIManager.main.HideAllDescriptionUI();
        UIManager.main.TurnOrderUI.Initialize(grid);
        UIManager.main.UnitUIViewer.Initialize(grid);
        RefreshLinkOutUI();
        SetTopBarOverlayEnabled(true);
        SetGeneralUIEnabled(false);
    }

    private void Update()
    {
        if (generalUI.enabled)
        {
            if(linkOutButton.interactable && Input.GetKeyDown(KeyCode.L))
            {
                OnLinkOutButton();
            }
            else if(endTurnButton.interactable && Input.GetKeyDown(KeyCode.E))
            {
                OnEndTurnButton();
            }
        }
        if (!inUnitSelection)
            return;
        for (int i = 0; i < MaxHotkeyIndex; ++i)
        {
            if (Input.GetKeyDown((i + 1).ToString()) && grid.TryGetHotKeyUnit(i, out var player))
            {
                SelectUnit(player.Pos, true);
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if(grid.TryGetHotKeyUnit(MaxHotkeyIndex, out var player))
            {
                SelectUnit(player.Pos, true);
                return;
            }
        }
    }

    private void EnterUnitSelection()
    {
        UnitSelectionUIEnabled = true;
        cursor.OnClick = SelectUnit;
        cursor.OnCancel = null;
        cursor.OnUnHighlight = UnHighlightUnit;
        cursor.OnHighlight = HighlightUnit;
    }

    private void HighlightUnit(Vector2Int pos)
    {
        if (!grid.TryGet(pos, out var unit))
        {
            return;
        }
        UIManager.main.UnitDescriptionUI.Show(unit);
        if (!unit.ShowUIByDefault)
        {
            unit.UI.SetVisible(true);
        }
    }

    private void UnHighlightUnit(Vector2Int pos)
    {
        UIManager.main.HideUnitDescriptionUI(pos);
        if (grid.TryGet(pos, out var unit) && !unit.ShowUIByDefault)
        {
            unit.UI.SetVisible(false);
        }
    }

    private void SelectUnit(Vector2Int pos)
    {
        SelectUnit(pos, false);
    }

    private void SelectUnit(Vector2Int pos, bool fromHotKey)
    {
        if (grid.TryGet(pos, out var unit) && unit.UnitTeam == Unit.Team.Player)
        {
            EnterActionMenu(unit, fromHotKey);
        }
    }

    private void EnterActionMenu(Unit unit, bool fromHotKey)
    {
        UIManager.main.UnitDescriptionUI.Hide();
        UnitSelectionUIEnabled = false;
        menu.Show(grid, this, unit, fromHotKey);
        cursor.OnClick = null;
        cursor.OnCancel = CancelActionMenu;
        cursor.OnUnHighlight = UnHighlightUnit;
        cursor.OnHighlight = HighlightUnit;
    }

    private void CancelActionMenu()
    {
        menu.Hide();
        UIManager.main.ActionDescriptionUI.Hide();
        EnterUnitSelection();
    }

    public void EnterActionUI(Action action, Unit unit)
    {
        int currAction = 0;
        lastTargets.Clear();
        lastSelectedPos = Vector2Int.zero;
        if (!ShowRangePattern(action, action.SubActions[currAction], unit, out var targetRangeEntries))
        {
            CancelTargetSelection(action, unit, ref currAction, ref targetRangeEntries);
            return;
        }
        UIManager.main.ActionDescriptionUI.Hide();
        cursor.OnCancel = () => CancelTargetSelection(action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnClick = (pos) => SelectActionTarget(pos, action, unit, ref currAction, ref targetRangeEntries);
        cursor.OnHighlight = (pos) => HighlightActionTarget(pos, action, unit, ref currAction);
        cursor.OnUnHighlight = (pos) => HideManyTiles(targetPatternEntries, true);
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
            HideManyTiles(entries, false);
            cursor.OnUnHighlight?.Invoke(unit.Pos);
            entries.Clear();
        }
        EnterActionMenu(unit, false);
    }

    private void HighlightActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction)
    {
        var subAction = action.SubActions[currAction];
        if (!GetRangePositions(action, subAction, unit).Contains(pos))
            return;
        targetPatternEntries.Clear();
        targetPatternEntries.AddRange(ShowTargetPattern(subAction.targetPattern, unit, pos));
    }


    private void SelectActionTarget(Vector2Int pos, Action action, Unit unit, ref int currAction, ref List<TileUI.Entry> entries)
    {
        var subAction = action.SubActions[currAction];
        if (!GetRangePositions(action, subAction, unit).Contains(pos))
            return;
        subAction.Use(grid, action, unit, pos, lastTargets, out var targets);
        lastTargets.Clear();
        lastTargets.AddRange(targets);
        lastSelectedPos = pos;
        HideManyTiles(entries, false);
        cursor.OnUnHighlight?.Invoke(pos);
        if (++currAction >= action.SubActions.Count)
        {
            FinishAction(action, unit);
            return;
        }
        if(!ShowRangePattern(action, action.SubActions[currAction], unit, out entries))
        {
            if(action.ActionType == Action.Type.Hybrid)
            {
                unit.ApplyHybridFailurePenaly(grid);
            }
            FinishAction(action, unit);
            return;
        }
        var currentPos = cursor.HighlightedPosition;
        if (grid.IsLegal(currentPos))
        {
            HighlightActionTarget(currentPos, action, unit, ref currAction);
        }
    }

    private void FinishAction(Action action, Unit unit)
    {
        action.FinishAction(grid, unit);
        if (!playerPhase.CheckEndBattle())
        {
            EnterUnitSelection();
            SaveManager.Save(SaveManager.State.Battle);
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
            if (grid.TryGet(pos, out var unit) && !unit.ShowUIByDefault)
            {
                unit.UI.SetVisible(true);
            }
        }
        return ret;
    }

    private IEnumerable<Vector2Int> GetRangePositions(Action action, SubAction subAction, Unit user)
    {
        var rangePos = subAction.OptionFlags.HasFlag(SubAction.Options.RangeBasedOnLastSelectorPos) ? lastSelectedPos : user.Pos;
        return subAction.GetValidRangePositions(grid, action, rangePos, user, lastTargets);
    }

    public bool ShowRangePattern(Action action, SubAction sub, Unit user, out List<TileUI.Entry> entries)
    {
        entries = new List<TileUI.Entry>();
        foreach (var pos in GetRangePositions(action, sub, user))
        {
            entries.Add(grid.SpawnTileUI(pos, TileUI.Type.RangePattern));
        }
        return entries.Count > 0;
    }

    public void HideManyTiles(IList<TileUI.Entry> entries, bool hideUnitUI)
    {
        foreach (var entry in entries)
        {
            if (hideUnitUI && grid.TryGet(entry.pos, out var unit) && !unit.ShowUIByDefault)
            {
                unit.UI.SetVisible(false);
            }
            grid.RemoveTileUI(entry);
        }
        entries.Clear();
    }

    #endregion
}
