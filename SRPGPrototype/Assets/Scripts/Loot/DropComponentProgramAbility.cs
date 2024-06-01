using System.Collections.Generic;

public class DropComponentProgramAbility : DropComponent<Program>
{
    public Loot<Program>.LootQuality[] lootQualities = new Loot<Program>.LootQuality[3];
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {

        return loot.GetDropsStandardNoDuplicates(lootQualities, Filter);
    }
    // Filter out all programs that don't give an ability or modifier effect
    static bool Filter(Program p) => p.GetComponent<ProgramEffectAddAbility>() != null || p.GetComponent<ProgramModifier>() != null;
}
