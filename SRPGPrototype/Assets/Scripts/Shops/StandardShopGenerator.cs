using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShopManager;

[CreateAssetMenu(fileName = "StandardShopGenerator", menuName = "ShopGenerators/Standard")]
public class StandardShopGenerator : ShopGenerator
{
    public override ShopData GenerateShopData(Transform objectContainer)
    {
        var loot = PersistantData.main.loot;
        var data = new ShopData();
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.GivesMoveAction), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.GivesWeaponAction), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.GivesRepositionSkill), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropsStandardNoDuplicates(LootProvider.LootQuality.Standard, 2, ProgramFilters.HasModifierEffect), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.HasAbility), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.GivesCapacity), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropsStandardNoDuplicates(LootProvider.LootQuality.High, 2), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropCustom(Rarity.Rare), objectContainer);
        return data;
    }
}
