﻿using System.Collections.Generic;

public class DropComponentShellStandard : DropComponent<Shell>
{
    public Loot<Shell>.LootQuality[] lootQualities = new Loot<Shell>.LootQuality[3];
    public override List<Shell> GenerateDrop(Loot<Shell> loot)
    {
        return loot.GetDropsStandardNoDuplicates(lootQualities);
    }
}
