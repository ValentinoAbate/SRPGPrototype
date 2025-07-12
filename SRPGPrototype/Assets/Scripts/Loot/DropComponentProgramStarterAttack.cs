using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropComponentProgramStarterAttack : DropComponent<Program>
{
    protected override List<Program> GenerateDrop(Loot<Program> loot)
    {
        var drops = loot.GetDropsStandardNoDuplicates(LootProvider.LootQuality.Low, 2, RandomWeaponFilter);
        drops.Insert(0, loot.GetDropCustom(Filter));
        return drops;
    }

    static bool RandomWeaponFilter(Program p) => ProgramFilters.IsRed(p) && ProgramFilters.GivesWeaponAction(p);

    // Filter out all programs that don't have the correct color and the proper attributes
    static bool Filter(Program p) => ProgramFilters.HasAttributes(p, Program.Attributes.Starter) && ProgramFilters.IsRed(p);
}
