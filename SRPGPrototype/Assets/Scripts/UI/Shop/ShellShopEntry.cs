using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShellShopEntry : ShopEntry
{
    public override string Name => shell.DisplayName;

    public override int Cost => shell.Rarity switch
    {
        Rarity.Common => 200,
        Rarity.Uncommon => 300,
        Rarity.Rare => 400,
        _ => 999
    };

    public override Color? IconColor => null;

    private readonly ShellDescriptionUI descriptionUI;
    private readonly Shell shell;
    private readonly ShopManager.ShopData shopData;

    public ShellShopEntry(ShopManager.ShopData shopData, Shell s, ShellDescriptionUI descriptionUI)
    {
        shell = s;
        this.descriptionUI = descriptionUI;
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
        if (descriptionUI == null)
            return;
        descriptionUI.Show(shell);
    }

    public override void OnHoverExit(BaseEventData data)
    {
        if (descriptionUI == null)
            return;
        descriptionUI.Hide();
    }
}
