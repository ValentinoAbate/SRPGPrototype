using Extensions.VectorIntDimensionUtils;
using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter Generation/Encounter Gen (Basic)")]
public class EncounterGeneratorBasic : EncounterGenerator
{
    [System.Flags]
    public enum LootModifiers
    { 
        None = 0,
        Shell = 1,
        BossCapacity = 2,
        Bonus = 4,
        NoNormalLoot = 8,
        BonusMoney = 16,
        BonusRandom = 32,
    }
    public enum MoneyOption
    {
        None,
        Normal,
        Miniboss,
        Boss,
    }

    public float targetDifficulty;
    public int numSpawnPositions = 3;
    [Header("Enemies")]
    public List<int> numEnemies = new List<int>{ 5 };
    public List<float> numEnemiesWeights = new List<float>{ 1 };
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public List<float> baseEnemyWeights = new List<float>() { 1 };
    [Header("Environment")]
    public List<int> numObstacles = new List<int> { 3 };
    public List<float> numObstaclesWeights = new List<float> { 1 };
    public List<ObstacleUnit> obstacles = new List<ObstacleUnit>();
    public List<float> baseObstacleWeights = new List<float>() { 1 };
    [Header("Loot")]
    public LootModifiers lootFlags;
    public MoneyOption moneyOption;
    [Header("Seed Properties")]
    public Encounter seed = null;

    public override Encounter Generate(string mapSymbol, int encounterNumber)
    {
        InitializeEncounter(mapSymbol, encounterNumber, out var positions, out var encounter);
        int numEnemiesChoice = RandomU.instance.Choice(numEnemies, numEnemiesWeights);
        int numObstaclesChoice = RandomU.instance.Choice(numObstacles, numObstaclesWeights);
        var enemySet = new WeightedSet<EnemyUnit>(enemies, baseEnemyWeights);
        var obstacleSet = new WeightedSet<ObstacleUnit>(obstacles, baseObstacleWeights);

        // Apply seed
        ApplySeed(seed, ref encounter, ref positions);

        // Generate enemies
        PlaceUnitsRandom(numEnemiesChoice / 2 + numEnemiesChoice % 2, enemySet, ref encounter, ref positions);
        PlaceUnitsWeighted(numEnemiesChoice / 2, enemySet, ClumpWeight, PassThrough, encounter, ref positions);

        // Generate obstacles
        PlaceUnitsRandom(numObstaclesChoice / 2 + numObstaclesChoice % 2, obstacleSet, ref encounter, ref positions);
        PlaceUnitsWeighted(numObstaclesChoice / 2, obstacleSet, ClumpWeight, PassThrough, encounter, ref positions);

        //Generate loot
        float difficulty = encounter.units.Where((e) => e.unit is IEncounterUnit)
                                               .Select((e) => e.unit as IEncounterUnit)
                                               .Sum((u) => u.EncounterData.challengeRating);
        // Calculate difficulty mod
        float difficultyMod = difficulty - targetDifficulty;

        // Generate Loot category weights
        var categoryWeights = new WeightedSet<MysteryDataUnit.Category>();
        var qualityWeights = new WeightedSet<MysteryDataUnit.Quality>();
        // Choose weights to use based on difficulty mod
        if (difficultyMod == 0)
        {
            categoryWeights.Add(lootCategoryWeights);
            qualityWeights.Add(lootQualityWeights);
        }
        else if (difficultyMod > 0)
        {
            categoryWeights.Add(lootCategoryWeightsDifficult);
            qualityWeights.Add(lootQualityWeightsDifficult);
        }
        else
        {
            categoryWeights.Add(lootCategoryWeightsEasy);
            qualityWeights.Add(lootQualityWeightsEasy);
        }
        // Place the default loot unless there shouldn't be normal loot 
        if (!lootFlags.HasFlag(EncounterGeneratorBasic.LootModifiers.NoNormalLoot))
        {
            // Actually place the loot
            PlaceLoot(categoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the bonus default loot (if applicable)
        if (lootFlags.HasFlag(EncounterGeneratorBasic.LootModifiers.Bonus))
        {
            PlaceLoot(categoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the bonus money loot (if applicable)
        if (lootFlags.HasFlag(EncounterGeneratorBasic.LootModifiers.BonusMoney))
        {
            PlaceLoot(moneyLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the bonus money or default loot (if applicable)
        if (lootFlags.HasFlag(EncounterGeneratorBasic.LootModifiers.BonusRandom))
        {
            var hybridWeights = new WeightedSet<MysteryDataUnit.Category>(categoryWeights);
            hybridWeights.Add(moneyLootCategoryWeights);
            PlaceLoot(hybridWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the shell loot (if applicable)
        if (lootFlags.HasFlag(EncounterGeneratorBasic.LootModifiers.Shell))
        {
            PlaceLoot(midbossLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the boss capacity loot (if applicable)
        if (lootFlags.HasFlag(EncounterGeneratorBasic.LootModifiers.BossCapacity))
        {
            PlaceLoot(bossLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Set additional spawn positions
        SetSpawnPositions(numSpawnPositions, encounter, ref positions);

        // Generate Money
        if (moneyOption == EncounterGeneratorBasic.MoneyOption.None)
        {
            encounter.giveCompletionMoney = false;
        }
        else
        {
            encounter.giveCompletionMoney = true;
            encounter.baseCompletionMoney = moneyOption switch
            {
                EncounterGeneratorBasic.MoneyOption.Miniboss => 50,
                EncounterGeneratorBasic.MoneyOption.Boss => 100,
                _ => 10,
            };
            encounter.completionMoneyVariance = moneyOption switch
            {
                EncounterGeneratorBasic.MoneyOption.Miniboss => 8,
                EncounterGeneratorBasic.MoneyOption.Boss => 12,
                _ => 4,
            };
        }

        return encounter;
    }
}
