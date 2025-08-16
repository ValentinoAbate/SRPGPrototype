using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CostButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CostDisplay costDisplay;

    private int cost;
    private bool? interactableOverride = null;

    public void SetCost(int cost)
    {
        this.cost = cost;
        bool canAfford = PersistantData.main.inventory.CanAfford(cost);
        costDisplay.Setup(cost, canAfford);
        UpdateButtonState(canAfford);
    }

    public void SetInteractable(bool interactable)
    {
        interactableOverride = interactable;
        button.interactable = UpdateButtonState();
    }

    public bool UpdateButtonState()
    {
        return UpdateButtonState(PersistantData.main.inventory.CanAfford(cost));
    }

    private bool UpdateButtonState(bool canAfford)
    {
        bool available = canAfford && (!interactableOverride.HasValue || interactableOverride.Value);
        button.interactable = available;
        costDisplay.UpdateCanAfford(available);
        return available;
    }
}
