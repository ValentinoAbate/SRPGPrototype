using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image costTextBacking;

    private int cost;
    private int index;
    private ShopUI shopUI;

    public void Show(ShopEntry entry, ShopUI shopUI, int index)
    {
        this.shopUI = shopUI;
        this.index = index;
        cost = entry.Cost;
        costText.text = $"${cost}";
        nameText.text = entry.Name;
        if(entry.IconColor.HasValue)
        {
            icon.color = entry.IconColor.Value;
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(entry.OnPurchase);
        button.onClick.AddListener(OnPurchase);
        trigger.SetHoverCallbacks(entry.OnHover, entry.OnHoverExit);
        if (RectTransformUtility.RectangleContainsScreenPoint(button.image.rectTransform, Input.mousePosition))
        {
            entry.OnHover(null);
        }
        RefreshInteractivity();
        gameObject.SetActive(true);
    }

    private void OnPurchase()
    {
        shopUI.OnPurchaseComplete(index);
    }

    public void RefreshInteractivity()
    {
        if (PersistantData.main.inventory.CanAfford(cost))
        {
            button.interactable = true;
            costTextBacking.color = Color.white;
        }
        else
        {
            button.interactable = false;
            costTextBacking.color = SharedColors.DisabledColor;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
