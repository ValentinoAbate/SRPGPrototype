using System.Collections.Generic;

public class DropComponentProgramColor: DropComponent<Program>
{
    public Program.Color color;
    public LootProvider.LootQuality[] lootQualities = new LootProvider.LootQuality[3];
    protected override List<Program> GenerateDrop(Loot<Program> loot)
    {

        return loot.GetDropsStandardNoDuplicates(lootQualities, Filter);
    }

    // Filter out all programs that aren't the correct color
    bool Filter(Program p) => ProgramFilters.IsColor(p, color);
}
