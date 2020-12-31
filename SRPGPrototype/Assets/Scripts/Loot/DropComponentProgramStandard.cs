using System.Collections.Generic;

public class DropComponentProgramStandard : DropComponent<Program>
{
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {
        return loot.GetDropsStandardNoDuplicates(lootQualities);
    }
}
