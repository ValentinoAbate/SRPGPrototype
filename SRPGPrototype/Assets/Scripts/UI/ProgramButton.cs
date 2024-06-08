using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(EventTrigger))]
public class ProgramButton : MonoBehaviour
{
    public TextMeshProUGUI buttonNameText;
    public Button button;
    public EventTrigger trigger;
    public Image icon;

    private void Awake()
    {
        button = GetComponent<Button>();
        trigger = GetComponent<EventTrigger>();
    }

    public void Initialize(Program p, CustUI uiManager)
    {
        icon.color = p.ColorValue;
        buttonNameText.text = p.DisplayName;

        button.onClick.AddListener(() => uiManager.PickupProgramFromButton(this, p));
        button.onClick.AddListener(() => button.interactable = false);
        trigger.triggers.Clear();
        var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
        hover.callback.AddListener((data) => uiManager.ShowProgramDescriptionWindow(p, false));
        var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
        hoverExit.callback.AddListener((data) => uiManager.HideProgramDescriptionWindow(false));
        trigger.triggers.Add(hover);
        trigger.triggers.Add(hoverExit);
    }

    public void Cancel()
    {
        button.interactable = true;
    }
}
