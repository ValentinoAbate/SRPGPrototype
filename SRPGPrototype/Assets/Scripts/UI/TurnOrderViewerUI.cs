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
    [SerializeField] private BattleUI battleUI;

    private PhaseManager phaseManager;

    public void Initialize(PhaseManager phaseManager, bool interactable)
    {
        this.phaseManager = phaseManager;
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
        SetInteractable(interactable);
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
        foreach(var unit in phaseManager.Units)
        {
            if(unit.UnitTeam == Unit.Team.Player)
            {
                if (battleUI.UnitSelectionUIEnabled)
                {
                    unit.UI.SetNumberActive(true);
                }
                continue;
            }
            if (unit.UnitTeam != Unit.Team.Enemy)
                continue;
            unit.UI.SetNumberActive(false);
        }
    }

    private void ShowTurnOrder(BaseEventData arg0)
    {
        int orderCounter = 0;
        for (int i = 0; i < phaseManager.Units.Count; i++)
        {
            var unit = phaseManager.Units[i];
            if (unit == null || unit.Dead)
                continue;
            if (unit.UnitTeam == Unit.Team.Player)
            {
                unit.UI.SetNumberActive(false);
                continue;
            }
            if (unit.UnitTeam != Unit.Team.Enemy)
                continue;
            unit.UI.SetNumberText((++orderCounter).ToString());
            unit.UI.SetNumberActive(true);
        }
    }
}
