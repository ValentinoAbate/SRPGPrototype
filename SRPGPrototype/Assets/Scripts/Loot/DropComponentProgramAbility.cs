using System.Collections.Generic;

public class DropComponentProgramAbility : DropComponent<Program>
{
    public LootProvider.LootQuality[] lootQualities = new LootProvider.LootQuality[3];
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {

        return loot.GetDropsStandardNoDuplicates(lootQualities, ProgramFilters.HasModifierOrAbility);
    }
}
