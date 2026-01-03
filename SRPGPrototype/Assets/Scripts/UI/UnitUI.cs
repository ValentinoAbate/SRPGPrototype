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

    [SerializeField] private Canvas canvas;

    public int Hp { set => hpNumberText.text = value.ToString(); }
    public int AP { set => apNumberText.text = value.ToString(); }

    public void SetVisible(bool visible)
    {
        canvas.enabled = visible;
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
