using System.Collections.Generic;

public class DropComponentProgramCapacity : DropComponent<Program>
{
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {
        return loot.GetDropsStandardNoDuplicates(lootQualities, Filter);
    }
    // Filter out all programs that don't give capacity
    static bool Filter(Program p) => p.GetComponent<ProgramEffectModifyCapacity>() != null;
}
