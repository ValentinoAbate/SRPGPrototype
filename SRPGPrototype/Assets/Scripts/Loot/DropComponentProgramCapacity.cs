﻿using System.Collections.Generic;

public class DropComponentProgramCapacity : DropComponent<Program>
{
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(LootManager manager)
    {
        // Filter out all programs that don't give capacity
        bool filter(Program p) => p.GetComponent<ProgramEffectModifyCapacity>() != null;
        return manager.ProgramLoot.GetDropsStandardNoDuplicates(lootQualities, filter);
    }

}
