using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropComponentBasicAttack : DropComponent<Program>
{
    private static readonly LootProvider.LootQuality[] qualities = new LootProvider.LootQuality[]
    {
        LootProvider.LootQuality.Low,
        LootProvider.LootQuality.Standard,
    };
    protected override List<Program> GenerateDrop(Loot<Program> loot)
    {
        var drops = loot.GetDropsStandardNoDuplicates(qualities, RandomWeaponFilter);
        return drops;
    }

    private static bool RandomWeaponFilter(Program p) => ProgramFilters.IsRed(p) && ProgramFilters.GivesWeaponAction(p);
}
