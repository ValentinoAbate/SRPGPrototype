using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ShellViewer : MonoBehaviour
{
    [SerializeField] private EventTrigger[] triggers;

    protected void ClearTriggers()
    {
        foreach (var trigger in triggers)
        {
            trigger.triggers.Clear();
        }
    }

    protected void AttachToShell(Shell s)
    {
        foreach (var trigger in triggers)
        {
            trigger.triggers.Clear();
            var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            hover.callback.AddListener((data) => UIManager.main.ShellDescriptionUI.Show(s));
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener(UIManager.main.HideShellDescriptionUI);
            trigger.triggers.Add(hover);
            trigger.triggers.Add(hoverExit);
        }
    }
}
