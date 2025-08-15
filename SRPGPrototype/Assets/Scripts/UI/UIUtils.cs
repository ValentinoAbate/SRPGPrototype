using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIUtils
{
    public static void SetCallback(this Button button, UnityAction callback)
    {
        button.onClick.RemoveAllListeners();
        if (callback != null)
            button.onClick.AddListener(callback);
    }

    public static void SetOnPointerEnter(this EventTrigger trigger, UnityAction<BaseEventData> onPointerEnter, UnityAction<BaseEventData> onPointerExit)
    {
        trigger.triggers.Clear();
        var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
        hover.callback.AddListener(onPointerEnter);
        var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
        hoverExit.callback.AddListener(onPointerExit);
        trigger.triggers.Add(hover);
        trigger.triggers.Add(hoverExit);
    }
}
