using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropComponentProgramStandard : DropComponent<Program>
{
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(LootManager manager)
    {
        return lootQualities.Select((q) => manager.ProgramLoot.GetDropStandard(q)).ToList();
    }
}
