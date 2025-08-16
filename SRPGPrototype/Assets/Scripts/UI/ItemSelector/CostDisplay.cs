using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image backing;

    private int cost = 0;
    public void Setup(int cost)
    {
        Setup(cost, PersistantData.main.inventory.CanAfford(cost));
    }

    public void Setup(int cost, bool canAfford)
    {
        this.cost = cost;
        if (cost > 0)
        {
            gameObject.SetActive(true);
            text.text = $"${cost}";
            UpdateCanAfford(canAfford);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdateCanAfford()
    {
        UpdateCanAfford(PersistantData.main.inventory.CanAfford(cost));
    }

    public void UpdateCanAfford(bool canAfford)
    {
        SetBackingColor(canAfford);
    }

    public void SetBackingColor(bool available)
    {
        backing.color = available ? Color.white : SharedColors.DisabledColor;
    }
}
