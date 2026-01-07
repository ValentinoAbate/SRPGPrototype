using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnOrderViewerUI : MonoBehaviour
{
    [SerializeField] private Image backingImage;
    [SerializeField] private EventTrigger[] triggers;
    [SerializeField] private Color disabledColor;

    private BattleGrid grid;

    private void Awake()
    {
        foreach (var trigger in triggers)
        {
            trigger.triggers.Clear();
            var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            hover.callback.AddListener(ShowTurnOrder);
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener(HideTurnOrder);
            trigger.triggers.Add(hover);
            trigger.triggers.Add(hoverExit);
        }
        SetInteractable(false);
    }

    public void Initialize(BattleGrid grid)
    {
        this.grid = grid;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetInteractable(bool interactable)
    {
        backingImage.color = interactable ? Color.white : disabledColor;
        foreach(var trigger in triggers)
        {
            trigger.enabled = interactable;
        }
    }

    private void HideTurnOrder(BaseEventData arg0)
    {
        foreach(var unit in grid)
        {
            if (unit.UnitTeam == Unit.Team.Player)
            {
                if (UIManager.main.BattleUI.UnitSelectionUIEnabled)
                {
                    unit.UI.SetNumberActive(true);
                }
            }
            else if (unit.PriorityLevel != Unit.Priority.Environment)
            {
                unit.UI.SetNumberActive(false);
            }
        }
    }

    private void ShowTurnOrder(BaseEventData arg0)
    {
        int orderCounter = 0;
        var units = new List<Unit>(grid);
        units.Sort();
        foreach (var unit in units)
        {
            if (unit == null || unit.Dead)
                continue;
            if (unit.UnitTeam == Unit.Team.Player)
            {
                unit.UI.SetNumberActive(false);
            }
            else if (unit.PriorityLevel != Unit.Priority.Environment)
            {
                unit.UI.SetNumberText((++orderCounter).ToString());
                unit.UI.SetNumberActive(true);
            }
        }
    }
}
