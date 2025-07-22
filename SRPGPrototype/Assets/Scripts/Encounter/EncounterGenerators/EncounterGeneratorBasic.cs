using Extensions.VectorIntDimensionUtils;
using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter Generation/Encounter Gen (Basic)")]
public class EncounterGeneratorBasic : EncounterGenerator
{
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
        var difficultyEnum = difficultyMod < 0 ? EncounterDifficulty.Easy : (difficultyMod > 0 ? EncounterDifficulty.Hard : EncounterDifficulty.Normal);
        // Choose weights to use based on difficulty mod
        GetLootWeights(difficultyEnum, out var categoryWeights, out var qualityWeights);
        PlaceLootDefault(lootFlags, categoryWeights, qualityWeights, ref encounter, ref positions);
        // Set additional spawn positions
        SetSpawnPositions(numSpawnPositions, encounter, ref positions);

        // Generate Money
        if (moneyOption == MoneyOption.None)
        {
            encounter.giveCompletionMoney = false;
        }
        else
        {
            encounter.giveCompletionMoney = true;
            encounter.baseCompletionMoney = moneyOption switch
            {
                MoneyOption.Miniboss => 50,
                MoneyOption.Boss => 100,
                _ => 10,
            };
            encounter.completionMoneyVariance = moneyOption switch
            {
                MoneyOption.Miniboss => 8,
                MoneyOption.Boss => 12,
                _ => 4,
            };
        }

        return encounter;
    }
}
