using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private Image colorIcon;
    [SerializeField] private TextMeshProUGUI text;
    public object Item { get; private set; }

    public void SetupAsProgram(Program p, UnityAction callback, Unit selector = null)
    {
        colorIcon.color = p.ColorValue;
        colorIcon.gameObject.SetActive(true);
        text.text = p.DisplayName;
        void ShowProgramDescriptionWindow(BaseEventData _)
        {
            UIManager.main.ProgramDescriptionUI.Show(p, selector);
        }
        trigger.SetOnPointerEnter(ShowProgramDescriptionWindow, UIManager.main.HideProgramDescriptionUI);
        button.SetCallback(callback);
        button.interactable = callback != null;
        Item = p;
        Show();
    }

    public void SetupAsShell(Shell s, UnityAction callback)
    {
        colorIcon.gameObject.SetActive(false);
        text.text = s.DisplayName;
        void ShowShellDescriptionWindow(BaseEventData _)
        {
            UIManager.main.ShellDescriptionUI.Show(s);
        }
        trigger.SetOnPointerEnter(ShowShellDescriptionWindow, UIManager.main.HideShellDescriptionUI);
        button.SetCallback(callback);
        button.interactable = callback != null;
        Item = s;
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
