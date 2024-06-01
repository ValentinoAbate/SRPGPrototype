using RandomUtils;
using System.Collections.Generic;
using System.Linq;

public class DropComponentProgramGamble : DropComponent<Program>
{
    private readonly WeightedSet<int> amountWeights = new WeightedSet<int>
    {
        {2, 50},
        {3, 20},
        {4, 15},
        {5, 10},
        {6, 4},
        {8, 1}
    };

    public override List<Program> GenerateDrop(Loot<Program> loot)
    {
        int roll = RandomU.instance.RandomInt(0, 1000);
        // Give the player a gamble program
        if (roll < 500)
        {
            // Filter out all programs that don't give capacity
            static bool GambleFilter(Program p) => p.attributes.HasFlag(Program.Attributes.Gamble);
            return new List<Program>() { loot.GetDropStandard(Loot<Program>.LootQuality.Even, GambleFilter) };
        }
        // Dud: give the player one standard drop
        if (roll < 650)
            return new List<Program>() { loot.GetDropStandard(Loot<Program>.LootQuality.Standard) };
        // Give the player a weighted random amount of standard drops 
        if (roll < 750)
            return loot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Standard, RandomU.instance.Choice(amountWeights));
        // Give the player a weighted random amount of High drops
        if (roll < 850)
            return loot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.High, RandomU.instance.Choice(amountWeights));
        // Give the player 3 equal changes at common / uncommon / rare
        if (roll < 950)
            return loot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Even, 3);
        // Very Rare: Give the player a boss program
        if (roll < 960)
            return new List<Program>() { loot.GetDropStandard(Loot<Program>.LootQuality.Boss) };
        // Very Rare: Give the player a capacity program
        if (roll < 970)
        {
            // Filter out all programs that don't give capacity
            static bool CapacityFilter(Program p) => p.GetComponent<ProgramEffectModifyCapacity>() != null;
            return new List<Program>() { loot.GetDropStandard(Loot<Program>.LootQuality.Even, CapacityFilter) };
        }
        // Very Rare: Give the player a white program
        if (roll < 980)
            return new List<Program>() { loot.GetDropStandard(Loot<Program>.LootQuality.Even, (p) => p.color == Program.Color.White) };
        // Very Rare: Give the player a random preinstall program
        if (roll < 990)
            return new List<Program> { loot.GetDropCustom(new WeightedSet<Rarity>() { Rarity.PreInstall }) };
        // Very Rare: Give the player a random unique program
        if (roll < 999)
            return new List<Program> { loot.GetDropCustom(new WeightedSet<Rarity>() { Rarity.Unique }) };
        // Extremely Rare: Give the player a program intended only for debugging
        if (roll < 1000)
            return new List<Program> { loot.GetDropCustom(new WeightedSet<Rarity>() { Rarity.Debug }) };
        throw new System.Exception("Roll should be in between 0 and 999");
    }

}
