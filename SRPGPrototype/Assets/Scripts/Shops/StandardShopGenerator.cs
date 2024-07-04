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
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropCustom(Rarity.Common, ProgramFilters.GivesMoveAction), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropCustom(Rarity.Common, ProgramFilters.GivesWeaponAction), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropCustom(Rarity.Common, ProgramFilters.GivesRepositionSkill), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.GivesRepositionSkill), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.HasModifierEffect), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.HasAbility), objectContainer);
        data.AddProgramFromAsset(loot.ProgramLoot.GetDropStandard(LootProvider.LootQuality.Standard, ProgramFilters.GivesCapacity), objectContainer);
        data.AddProgramsFromAssets(loot.ProgramLoot.GetDropsStandardNoDuplicates(LootProvider.LootQuality.High, 3), objectContainer);
        data.AddProgramsFromAssets(loot.ProgramLoot.GetDropsCustomNoDuplicates(Rarity.Rare, 2), objectContainer);
        return data;
    }
}
