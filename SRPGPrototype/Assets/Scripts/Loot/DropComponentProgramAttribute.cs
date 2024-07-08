using System.Collections.Generic;

public class DropComponentProgramAttribute : DropComponent<Program>
{
    public Program.Attributes attributes = Program.Attributes.None;
    public LootProvider.LootQuality[] lootQualities = new LootProvider.LootQuality[3];
    protected override List<Program> GenerateDrop(Loot<Program> loot)
    {
        return loot.GetDropsStandardNoDuplicates(lootQualities);
    }

    // Filter out all programs that don't give the given attributes
    bool Filter(Program p) => p.attributes.HasFlag(attributes);
}
