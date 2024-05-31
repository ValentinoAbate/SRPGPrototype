using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpNumberText = null;
    [SerializeField] private TextMeshProUGUI apNumberText = null;

    [SerializeField] private Transform hotKeyUI = null;
    [SerializeField] private TextMeshProUGUI hotKeyText = null;

    public int Hp { set => hpNumberText.text = value.ToString(); }
    public int AP { set => apNumberText.text = value.ToString(); }

    public void SetHotKey(string text)
    {
        if (hotKeyText == null)
            return;
        hotKeyText.text = text;
    }

    public void SetHotKeyActive(bool active)
    {
        if (hotKeyUI == null)
            return;
        hotKeyUI.gameObject.SetActive(active);
    }
}
