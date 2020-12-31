using System.Collections.Generic;

public class DropComponentProgramAbility : DropComponent<Program>
{
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {
        // Filter out all programs that don't give capacity
        static bool Filter(Program p) => p.GetComponent<ProgramEffectAddAbility>() != null || p.GetComponent<ProgramModifier>() != null;
        return loot.GetDropsStandardNoDuplicates(lootQualities, Filter);
    }

}
