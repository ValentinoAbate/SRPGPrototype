using Extensions.VectorIntDimensionUtils;
using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterNew", menuName = "Event Data / Encounter Data")]
public class EncounterData : EncounterGenerator
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
    public enum Type 
    {
        Encounter,
        Shop,
        Boss,
        Rest,
    }

    [Header("General")]
    public float targetDifficulty;
    public Vector2Int dimensions = new Vector2Int(8, 8);
    public int numSpawnPositions = 3;
    public Type encounterType;
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
        var positions = dimensions.Enumerate();
        var encounter = new Encounter() { nameOverride = $"{encounterType} {mapSymbol}-{encounterNumber}", dimensions = dimensions };
        int numEnemiesChoice = RandomU.instance.Choice(numEnemies, numEnemiesWeights);
        int numObstaclesChoice = RandomU.instance.Choice(numObstacles, numObstaclesWeights);
        var enemySet = new WeightedSet<EnemyUnit>(enemies, baseEnemyWeights);
        var obstacleSet = new WeightedSet<ObstacleUnit>(obstacles, baseObstacleWeights);

        // Initialize encounter values from seed if applicable
        if (seed != null)
        {
            List<Encounter.UnitEntry> outOfBoundsUnits = new List<Encounter.UnitEntry>();
            foreach (var entry in seed.units)
            {
                // Position is invalid, generated a proper position later
                if (!positions.Contains(entry.pos))
                {
                    outOfBoundsUnits.Add(entry);
                    continue;
                }
                encounter.units.Add(entry);
                positions.Remove(entry.pos);
            }
            // Remove all seed spawn positions from valid positions choices
            positions.RemoveAll((p) => seed.spawnPositions.Contains(p));
            // Add all seed spawn positions to the encounter
            encounter.spawnPositions.AddRange(seed.spawnPositions);
            // Generate positions for any unit entries that had negative positions
            foreach (var entry in outOfBoundsUnits)
            {
                if (positions.Count <= 0)
                    break;
                Vector2Int pos = RandomU.instance.Choice(positions);
                encounter.units.Add(new Encounter.UnitEntry(entry.unit, pos));
                positions.Remove(pos);
            }
            if (!string.IsNullOrEmpty(seed.nameOverride))
            {
                encounter.nameOverride = seed.nameOverride;
            }
        }

        // Generate enemies
        PlaceUnitsRandom(numEnemiesChoice / 2 + numEnemiesChoice % 2, enemySet, ref encounter, ref positions);
        PlaceUnitsWeighted(numEnemiesChoice / 2, enemySet, ClumpWeight, PassThrough, dimensions, encounter, ref positions);

        // Generate obstacles
        PlaceUnitsRandom(numObstaclesChoice / 2 + numObstaclesChoice % 2, obstacleSet, ref encounter, ref positions);
        PlaceUnitsWeighted(numObstaclesChoice / 2, obstacleSet, ClumpWeight, PassThrough, dimensions, encounter, ref positions);

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
        if (!lootFlags.HasFlag(EncounterData.LootModifiers.NoNormalLoot))
        {
            // Actually place the loot
            PlaceLoot(categoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the bonus default loot (if applicable)
        if (lootFlags.HasFlag(EncounterData.LootModifiers.Bonus))
        {
            PlaceLoot(categoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the bonus money loot (if applicable)
        if (lootFlags.HasFlag(EncounterData.LootModifiers.BonusMoney))
        {
            PlaceLoot(moneyLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the bonus money or default loot (if applicable)
        if (lootFlags.HasFlag(EncounterData.LootModifiers.BonusRandom))
        {
            var hybridWeights = new WeightedSet<MysteryDataUnit.Category>(categoryWeights);
            hybridWeights.Add(moneyLootCategoryWeights);
            PlaceLoot(hybridWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the shell loot (if applicable)
        if (lootFlags.HasFlag(EncounterData.LootModifiers.Shell))
        {
            PlaceLoot(midbossLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the boss capacity loot (if applicable)
        if (lootFlags.HasFlag(EncounterData.LootModifiers.BossCapacity))
        {
            PlaceLoot(bossLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Set additional spawn positions
        SetSpawnPositions(numSpawnPositions, dimensions, encounter, ref positions);

        // Generate Money
        if (moneyOption == EncounterData.MoneyOption.None)
        {
            encounter.giveCompletionMoney = false;
        }
        else
        {
            encounter.giveCompletionMoney = true;
            encounter.baseCompletionMoney = moneyOption switch
            {
                EncounterData.MoneyOption.Miniboss => 50,
                EncounterData.MoneyOption.Boss => 100,
                _ => 10,
            };
            encounter.completionMoneyVariance = moneyOption switch
            {
                EncounterData.MoneyOption.Miniboss => 8,
                EncounterData.MoneyOption.Boss => 12,
                _ => 4,
            };
        }

        return encounter;
    }
}
