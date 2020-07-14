using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI hpNumberText;
    [SerializeField]
    private TextMeshProUGUI apNumberText;

    public int Hp { set => hpNumberText.text = value.ToString(); }
    public int AP { set => apNumberText.text = value.ToString(); }
}
