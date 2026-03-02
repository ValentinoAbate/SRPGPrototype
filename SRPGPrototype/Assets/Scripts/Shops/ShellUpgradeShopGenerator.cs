using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShopManager;

[CreateAssetMenu(fileName = "ShellUpgradeShopGenerator", menuName = "ShopGenerators/Shell Upgrade")]
public class ShellUpgradeShopGenerator : ShopGenerator
{
    public override ShopData GenerateShopData(Transform objectContainer)
    {
        var loot = PersistantData.main.loot;
        var data = new ShopData();
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropsStandard(LootProvider.LootQuality.Standard, 3, ProgramFilters.GivesCapacity), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropsCustom(Rarity.ShopShellUpgrade, 3, ProgramFilters.IsExpander), objectContainer);
        return data;
    }
}
