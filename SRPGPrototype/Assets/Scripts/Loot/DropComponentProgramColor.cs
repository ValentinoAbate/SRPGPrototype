using System.Collections.Generic;

public class DropComponentProgramColor: DropComponent<Program>
{
    public Program.Color color;
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(LootManager manager)
    {
        // Filter out all programs that don't give capacity
        bool filter(Program p) => p.color == color;
        return manager.ProgramLoot.GetDropsStandard(lootQualities, filter);
    }

}
