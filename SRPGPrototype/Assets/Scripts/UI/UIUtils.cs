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
}
