using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShellShopEntry : ShopEntry
{
    public override string Name => shell.DisplayName;

    public override int Cost => shell.Rarity switch
    {
        Rarity.Common => 150,
        Rarity.Uncommon => 200,
        Rarity.Rare => 250,
        _ => 999
    };

    public override Color? IconColor => null;

    private readonly Shell shell;
    private readonly ShopManager.ShopData shopData;

    public ShellShopEntry(ShopManager.ShopData shopData, Shell s)
    {
        shell = s;
        this.shopData = shopData;
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
