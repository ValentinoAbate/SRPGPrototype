using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShellShopEntry : ShopEntry
{
    public override string Name => shell.DisplayName;
    public override int Cost => cost;
    public override Color? IconColor => null;

    private readonly Shell shell;
    private readonly ShopManager.ShopData shopData;
    private readonly int cost;

    public ShellShopEntry(ShopManager.ShopData shopData, Shell s)
    {
        shell = s;
        this.shopData = shopData;
        cost = GetCost(shopData, s);
    }

    private int GetCost(ShopManager.ShopData shopData, Shell s)
    {
        return Mathf.CeilToInt((shopData.CostPercentMultiplier / 100f) * (shell.Rarity switch
        {
            Rarity.Common => 150,
            Rarity.Uncommon => 200,
            Rarity.Rare => 250,
            _ => 999
        }));
    }

    public override void OnPurchase()
    {
        base.OnPurchase();
        PersistantData.main.inventory.AddShell(shell);
        shopData.RemoveShell(shell);
    }

    public override void OnHover(BaseEventData data)
    {
        UIManager.main.ShellDescriptionUI.Show(shell);
    }

    public override void OnHoverExit(BaseEventData data)
    {
        UIManager.main.ShellDescriptionUI.Hide();
    }
}
