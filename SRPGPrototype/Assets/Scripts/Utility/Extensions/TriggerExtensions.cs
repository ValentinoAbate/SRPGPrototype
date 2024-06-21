using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class TriggerExtensions
{
    public static void SetHoverCallbacks(this EventTrigger trigger, UnityAction<BaseEventData> onHover, UnityAction<BaseEventData> onHoverExit, bool clear = true)
    {
        // Set up event triggers
        if (clear)
        {
            trigger.triggers.Clear();
        }
        var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
        hover.callback.AddListener(onHover);
        var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
        hoverExit.callback.AddListener(onHoverExit);
        trigger.triggers.Add(hover);
        trigger.triggers.Add(hoverExit);
    }
}
