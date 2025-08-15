using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Show(int cost)
    {
        text.text = $"${cost}";
    }
}
