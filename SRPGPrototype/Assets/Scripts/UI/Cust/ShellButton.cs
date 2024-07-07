using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShellButton : MonoBehaviour
{

    public TextMeshProUGUI shellNameText;
    public Image shellImage;
    public Button equipButton;
    public Button custButton;
    public EventTrigger[] triggers;
    public Color compiledColor = Color.green;
    public Color unCompiledColor = Color.red;
    public Color soulCoreColor = new Color(255, 165, 0);

    public void Initialize(Shell s, CustUI uiManager, bool equippedShell = false)
    {
        shellNameText.text = s.DisplayName;
        equipButton.onClick.AddListener(() => uiManager.EquipShell(s));
        custButton.onClick.AddListener(() => uiManager.EnterCust(s));
        shellImage.color = s.Compiled ? s.HasSoulCore ? soulCoreColor : compiledColor : unCompiledColor;
        equipButton.interactable = !equippedShell && s.Compiled && !s.HasSoulCore;
        foreach(var trigger in triggers)
        {
            trigger.triggers.Clear();
            var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            hover.callback.AddListener((data) => uiManager.shellDescriptionUI.Show(s));
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener((data) => uiManager.shellDescriptionUI.Hide());
            trigger.triggers.Add(hover);
            trigger.triggers.Add(hoverExit);
        }
    }
}
