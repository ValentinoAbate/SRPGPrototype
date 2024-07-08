using System.Collections.Generic;

public class DropComponentProgramStandard : DropComponent<Program>
{
    public LootProvider.LootQuality[] lootQualities = new LootProvider.LootQuality[3];
    protected override List<Program> GenerateDrop(Loot<Program> loot)
    {
        return loot.GetDropsStandardNoDuplicates(lootQualities);
    }
}
