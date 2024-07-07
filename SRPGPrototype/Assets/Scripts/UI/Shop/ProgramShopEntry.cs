using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProgramShopEntry : ShopEntry
{
    public override string Name => program.DisplayName;

    public override int Cost => program.Rarity switch
    {
        Rarity.Common => 100,
        Rarity.Uncommon => 150,
        Rarity.Rare => 300,
        _ => 999,
    };

    public override Color? IconColor => program.ColorValue;

    private readonly Program program;
    private readonly ShopManager.ShopData shopData;

    public ProgramShopEntry(ShopManager.ShopData shopData, Program program)
    {
        this.program = program;
        this.shopData = shopData;
    }

    public override void OnPurchase()
    {
        base.OnPurchase();
        PersistantData.main.inventory.AddProgram(program, false);
        shopData.RemoveProgram(program);
    }

    public override void OnHover(BaseEventData data)
    {
        UIManager.main.ProgramDescriptionUI.Show(program);
    }

    public override void OnHoverExit(BaseEventData data)
    {
        UIManager.main.ProgramDescriptionUI.Hide();
    }
}
