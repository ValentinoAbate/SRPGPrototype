using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShopManager;

[CreateAssetMenu(fileName = "RandomShopGenerator", menuName = "ShopGenerators/Random")]
public class RandomShopGenerator : ShopGenerator
{
    public override ShopData GenerateShopData(Transform objectContainer)
    {
        var loot = PersistantData.main.loot;
        var data = new ShopData();
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropsStandardNoDuplicates(LootProvider.LootQuality.Standard, 3), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropsStandard(LootProvider.LootQuality.High, 1), objectContainer);
        return data;
    }
}
