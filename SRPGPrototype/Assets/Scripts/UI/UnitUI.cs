using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpNumberText = null;
    [SerializeField] private TextMeshProUGUI apNumberText = null;

    [SerializeField] private Transform numberUI = null;
    [SerializeField] private TextMeshProUGUI numberText = null;
    [SerializeField] private StatIconUI breakUI;
    [SerializeField] private StatIconUI powerUI;

    [SerializeField] private CanvasGroup mainUI;

    public int Hp { set => hpNumberText.text = value.ToString(); }
    public int AP { set => apNumberText.text = value.ToString(); }
    public int Break { set => breakUI.SetValue(value); }
    public int Power { set => powerUI.SetValue(value); }

    public void SetVisible(bool visible)
    {
        mainUI.alpha = visible ? 1 : 0;
    }

    public void SetNumberText(string text)
    {
        if (numberText == null)
            return;
        numberText.text = text;
    }

    public void SetNumberActive(bool active)
    {
        if (numberUI == null)
            return;
        numberUI.gameObject.SetActive(active);
    }
}
