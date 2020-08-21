using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropComponentProgramCapacity : DropComponent<Program>
{
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(LootManager manager)
    {
        // Filter out all programs that don't give capacity
        bool filter(Program p) => p.GetComponent<ProgramEffectModifyCapacity>() != null;
        return lootQualities.Select((q) => manager.ProgramLoot.GetDropStandard(q, filter)).ToList();
    }

}
