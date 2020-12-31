using System.Collections.Generic;

public class DropComponentProgramAttribute : DropComponent<Program>
{
    public Program.Attributes attributes = Program.Attributes.None;
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {
        // Filter out all programs that don't give the given attributes
        bool Filter(Program p) => p.attributes.HasFlag(attributes);
        return loot.GetDropsStandardNoDuplicates(lootQualities, Filter);
    }

}
