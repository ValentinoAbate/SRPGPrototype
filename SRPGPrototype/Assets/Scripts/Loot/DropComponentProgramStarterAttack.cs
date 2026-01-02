using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropComponentProgramStarterAttack : DropComponent<Program>
{
    protected override List<Program> GenerateDrop(Loot<Program> loot)
    {
        return loot.GetDropsCustomNoDuplicates(2, Filter);
    }

    // Must be a starter program that gives a weapon action
    private static bool Filter(Program p) => ProgramFilters.HasAttributes(p, Program.Attributes.Starter) && ProgramFilters.GivesWeaponAction(p);
}
