using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropComponentShellStandard : DropComponent<Shell>
{
    public Loot<Shell>.LootQuality[] lootQualities = new Loot<Shell>.LootQuality[3];
    public override List<Shell> GenerateDrop(LootManager manager)
    {
        return lootQualities.Select((q) => manager.ShellLoot.GetDropStandard(q)).ToList();
    }
}
