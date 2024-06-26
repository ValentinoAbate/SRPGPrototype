using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ShopEntry
{
    public abstract string Name { get; }
    public abstract int Cost { get; }
    public abstract Color? IconColor { get; }

    public virtual void OnPurchase()
    {
        PersistantData.main.inventory.Money -= Cost;
    }
    public abstract void OnHover(BaseEventData data);
    public abstract void OnHoverExit(BaseEventData data);
}
