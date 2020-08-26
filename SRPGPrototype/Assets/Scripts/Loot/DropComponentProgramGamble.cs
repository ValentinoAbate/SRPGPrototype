using RandomUtils;
using System.Collections.Generic;

public class DropComponentProgramGamble : DropComponent<Program>
{
    private WeightedSet<int> amountWeights = new WeightedSet<int>
    {
        {2, 50},
        {3, 20},
        {4, 15},
        {5, 10},
        {6, 4},
        {8, 1}
    };

    public override List<Program> GenerateDrop(LootManager manager)
    {
        int roll = RandomU.instance.RandomInt(0, 1000);
        // Dud: give the player one standard drop
        if(roll < 400)
            return manager.ProgramLoot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Standard, 1);
        // Give the player a weighted random amount of standard drops 
        if (roll < 750)
            return manager.ProgramLoot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Standard, RandomU.instance.Choice(amountWeights));
        // Give the player a weighted random amount of High drops
        if (roll < 850)
            return manager.ProgramLoot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.High, RandomU.instance.Choice(amountWeights));
        // Give the player 3 equal changes at common / uncommon / rare
        if (roll < 950)
            return manager.ProgramLoot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Even, 3);
        // Very Rare: Give the player a boss program
        if (roll < 960)
            return manager.ProgramLoot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Boss, 1);
        // Very Rare: Give the player a capacity program
        if (roll < 970)
        {
            // Filter out all programs that don't give capacity
            bool capacityFilter(Program p) => p.GetComponent<ProgramEffectModifyCapacity>() != null;
            return manager.ProgramLoot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Even, 1, capacityFilter);
        }
        // Very Rare: Give the player a white program
        if (roll < 980)
            return manager.ProgramLoot.GetDropsStandardNoDuplicates(Loot<Program>.LootQuality.Even, 1, (p) => p.color == Program.Color.White);
        // Very Rare: Give the player a random preinstall program
        if (roll < 990)
            return new List<Program> { manager.ProgramLoot.GetDropCustom(new WeightedSet<Rarity>() { Rarity.PreInstall }) };
        // Very Rare: Give the player a random unique program
        if (roll < 999)
            return new List<Program> { manager.ProgramLoot.GetDropCustom(new WeightedSet<Rarity>() { Rarity.Unique }) };
        // Extremely Rare: Give the player a program intended only for debugging
        if (roll < 1000)
            return new List<Program> { manager.ProgramLoot.GetDropCustom(new WeightedSet<Rarity>() { Rarity.Debug }) };
        throw new System.Exception("Roll should be in between 0 and 999");
    }

}
