using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProgramShopEntry : ShopEntry
{
    public override string Name => program.DisplayName;

    public override int Cost => program.Rarity switch
    {
        Rarity.Common => 75,
        Rarity.Uncommon => 125,
        Rarity.Rare => 250,
        Rarity.ShopShellUpgrade => 125,
        _ => 999,
    };

    public override Color? IconColor => program.ColorValue;

    private readonly Program program;
    private readonly ShopManager.ShopData shopData;
    private readonly Unit shopper;

    public ProgramShopEntry(ShopManager.ShopData shopData, Program program, Unit shopper)
    {
        this.program = program;
        this.shopData = shopData;
        this.shopper = shopper;
    }

    public override void OnPurchase()
    {
        base.OnPurchase();
        PersistantData.main.inventory.AddProgram(program);
        shopData.RemoveProgram(program);
    }

    public override void OnHover(BaseEventData data)
    {
        UIManager.main.ProgramDescriptionUI.Show(program, shopper);
    }

    public override void OnHoverExit(BaseEventData data)
    {
        UIManager.main.ProgramDescriptionUI.Hide();
    }
}
