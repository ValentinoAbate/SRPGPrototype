using System.Collections.Generic;

public class DropComponentProgramColor: DropComponent<Program>
{
    public Program.Color color;
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {

        return loot.GetDropsStandardNoDuplicates(lootQualities, Filter);
    }

    // Filter out all programs that aren't the correct color
    bool Filter(Program p) => p.color == color && p.GetComponent<ProgramVariantColor>() == null;
}
