using System.Collections.Generic;

public class DropComponentProgramAttribute : DropComponent<Program>
{
    public Program.Attributes attributes = Program.Attributes.None;
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(LootManager manager)
    {
        // Filter out all programs that don't give capacity
        bool filter(Program p) => p.attributes.HasFlag(attributes);
        return manager.ProgramLoot.GetDropsStandardNoDuplicates(lootQualities, filter);
    }

}
