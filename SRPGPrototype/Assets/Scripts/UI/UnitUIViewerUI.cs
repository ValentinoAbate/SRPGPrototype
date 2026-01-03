using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitUIViewerUI : MonoBehaviour
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
            hover.callback.AddListener(ShowUI);
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener(HideUI);
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

    public void HideUI() => HideUI(null);
    private void HideUI(BaseEventData arg0)
    {
        SetUIVisible(false);
    }

    private void ShowUI(BaseEventData arg0)
    {
        SetUIVisible(true);
    }

    private void SetUIVisible(bool visible)
    {
        foreach (var unit in grid)
        {
            if (!unit.ShowUIByDefault)
            {
                unit.UI.SetVisible(visible);
            }
        }
    }
}
