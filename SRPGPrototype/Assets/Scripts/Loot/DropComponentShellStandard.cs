using System.Collections.Generic;

public class DropComponentShellStandard : DropComponent<Shell>
{
    public LootProvider.LootQuality[] lootQualities = new LootProvider.LootQuality[3];
    protected override List<Shell> GenerateDrop(Loot<Shell> loot)
    {
        return loot.GetDropsStandardNoDuplicates(lootQualities);
    }
}
