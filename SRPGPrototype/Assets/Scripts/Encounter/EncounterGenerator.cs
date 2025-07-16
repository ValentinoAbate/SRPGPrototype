using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions.VectorIntDimensionUtils;

public class EncounterGenerator : MonoBehaviour
{
    public delegate float NextPosWeighting(Vector2Int pos, Encounter encounter, Vector2Int dimensions);
    public delegate float NextUnitWeighting(Unit u, Encounter encounter, Vector2Int dimensions);

    #region Weightings

    private readonly WeightedSet<MysteryDataUnit.Category> lootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Standard, 72 },
        { MysteryDataUnit.Category.Color, 24 },
        { MysteryDataUnit.Category.Ability, 3 },
        { MysteryDataUnit.Category.Shell, 0.1f },
        { MysteryDataUnit.Category.Capacity, 0.1f },
        { MysteryDataUnit.Category.Gamble,  0.8f },
    };
    private readonly WeightedSet<MysteryDataUnit.Category> lootCategoryWeightsDifficult = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Standard, 50 },
        { MysteryDataUnit.Category.Color, 30 },
        { MysteryDataUnit.Category.Ability, 10 },
        { MysteryDataUnit.Category.Shell, 1 },
        { MysteryDataUnit.Category.Capacity, 1 },
        { MysteryDataUnit.Category.Gamble,  8 },
    };
    private readonly WeightedSet<MysteryDataUnit.Category> lootCategoryWeightsEasy = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Standard, 85 },
        { MysteryDataUnit.Category.Color, 14 },
        { MysteryDataUnit.Category.Gamble,  1 },
    };
    private readonly WeightedSet<MysteryDataUnit.Quality> lootQualityWeights = new WeightedSet<MysteryDataUnit.Quality>
    {
        { MysteryDataUnit.Quality.Common, 89 },
        { MysteryDataUnit.Quality.Uncommon, 10 },
        { MysteryDataUnit.Quality.Rare, 1 },
    };
    private readonly WeightedSet<MysteryDataUnit.Quality> lootQualityWeightsDifficult = new WeightedSet<MysteryDataUnit.Quality>
    {
        { MysteryDataUnit.Quality.Common, 60 },
        { MysteryDataUnit.Quality.Uncommon, 35 },
        { MysteryDataUnit.Quality.Rare, 5 },
    };
    private readonly WeightedSet<MysteryDataUnit.Quality> lootQualityWeightsEasy = new WeightedSet<MysteryDataUnit.Quality>
    {
        { MysteryDataUnit.Quality.Common, 95 },
        { MysteryDataUnit.Quality.Uncommon, 4.75f },
        { MysteryDataUnit.Quality.Rare, 0.25f },
    };
    private readonly WeightedSet<MysteryDataUnit.Category> midbossLootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Shell, 100 }
    };
    private readonly WeightedSet<MysteryDataUnit.Category> bossLootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Boss, 100 }
    };
    private readonly WeightedSet<MysteryDataUnit.Category> moneyLootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Money, 100 },
    };

    #endregion

    private readonly Dictionary<MysteryDataUnit.Category, Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>> lootUnitTable 
        = new Dictionary<MysteryDataUnit.Category, Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>>();

    [SerializeField] private List<MysteryDataUnit> lootUnits = new List<MysteryDataUnit>();

    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized)
            return;
        initialized = true;
        lootUnitTable.Clear();
        foreach(var unit in lootUnits)
        {
            if (!lootUnitTable.ContainsKey(unit.LootCategory))
                lootUnitTable.Add(unit.LootCategory, new Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>());
            if (!lootUnitTable[unit.LootCategory].ContainsKey(unit.LootQuality))
                lootUnitTable[unit.LootCategory].Add(unit.LootQuality, new List<MysteryDataUnit>() { unit });
            else
                lootUnitTable[unit.LootCategory][unit.LootQuality].Add(unit);
        }
    }

    public Encounter Generate(EncounterData data, int mapIndex, int index)
    {
        Initialize();
        var positions = data.dimensions.Enumerate();
        var encounter = new Encounter() { nameOverride = $"{data.encounterType} {mapIndex}-{index}", dimensions = data.dimensions };
        int numSpawnPositions = data.numSpawnPositions;
        int numEnemies = RandomU.instance.Choice(data.numEnemies, data.numEnemiesWeights);
        int numObstacles = RandomU.instance.Choice(data.numObstacles, data.numObstaclesWeights);
        var enemies = new WeightedSet<EnemyUnit>(data.enemies, data.baseEnemyWeights);
        var obstacles = new WeightedSet<ObstacleUnit>(data.obstacles, data.baseObstacleWeights);
        var lootFlags = data.lootFlags;

        // Initialize encounter values from seed if applicable
        if (data.seed != null)
        {
            List<Encounter.UnitEntry> outOfBoundsUnits = new List<Encounter.UnitEntry>();
            foreach(var entry in data.seed.units)
            {
                // Position is invalid, generated a proper position later
                if(!positions.Contains(entry.pos))
                {
                    outOfBoundsUnits.Add(entry);
                    continue;
                }
                encounter.units.Add(entry);
                positions.Remove(entry.pos);
            }
            // Remove all seed spawn positions from valid positions choices
            positions.RemoveAll((p) => data.seed.spawnPositions.Contains(p));
            // Add all seed spawn positions to the encounter
            encounter.spawnPositions.AddRange(data.seed.spawnPositions);
            // Generate positions for any unit entries that had negative positions
            foreach(var entry in outOfBoundsUnits)
            {
                if (positions.Count <= 0)
                    break;
                Vector2Int pos = RandomU.instance.Choice(positions);
                encounter.units.Add(new Encounter.UnitEntry(entry.unit, pos));
                positions.Remove(pos);
            }
            if (!string.IsNullOrEmpty(data.seed.nameOverride))
            {
                encounter.nameOverride = data.seed.nameOverride;
            }
        }

        // Generate enemies
        PlaceUnitsRandom(numEnemies / 2 + numEnemies % 2, enemies, ref encounter, ref positions);
        PlaceUnitsWeighted(numEnemies / 2, enemies, ClumpWeight, PassThrough, data.dimensions, encounter, ref positions);

        // Generate obstacles
        PlaceUnitsRandom(numObstacles / 2 + numObstacles % 2, obstacles, ref encounter, ref positions);
        PlaceUnitsWeighted(numObstacles / 2, obstacles, ClumpWeight, PassThrough, data.dimensions, encounter, ref positions);

        //Generate loot
        float difficulty = encounter.units.Where((e) => e.unit is IEncounterUnit)
                                               .Select((e) => e.unit as IEncounterUnit)
                                               .Sum((u) => u.EncounterData.challengeRating);
        // Calculate difficulty mod
        float difficultyMod = difficulty - data.targetDifficulty;

        // Generate Loot category weights
        var categoryWeights = new WeightedSet<MysteryDataUnit.Category>();
        var qualityWeights = new WeightedSet<MysteryDataUnit.Quality>();
        // Choose weights to use based on difficulty mod
        if(difficultyMod == 0)
        {
            categoryWeights.Add(lootCategoryWeights);
            qualityWeights.Add(lootQualityWeights);
        }
        else if(difficultyMod > 0)
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
        if(!lootFlags.HasFlag(EncounterData.LootModifiers.NoNormalLoot))
        {
            // Actually place the loot
            PlaceLoot(categoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Place the bonus default loot (if applicable)
        if(lootFlags.HasFlag(EncounterData.LootModifiers.Bonus))
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
        if(lootFlags.HasFlag(EncounterData.LootModifiers.BossCapacity))
        {
            PlaceLoot(bossLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        // Set additional spawn positions
        SetSpawnPositions(numSpawnPositions, data.dimensions, encounter, ref positions);

        // Generate Money
        if(data.moneyOption == EncounterData.MoneyOption.None)
        {
            encounter.giveCompletionMoney = false;
        }
        else
        {
            encounter.giveCompletionMoney = true;
            encounter.baseCompletionMoney = data.moneyOption switch
            {
                EncounterData.MoneyOption.Miniboss => 50,
                EncounterData.MoneyOption.Boss => 100,
                _ => 10,
            };
            encounter.completionMoneyVariance = data.moneyOption switch
            {
                EncounterData.MoneyOption.Miniboss => 8,
                EncounterData.MoneyOption.Boss => 12,
                _ => 4,
            };
        }

        return encounter;
    }

    private void PlaceUnitsWeighted<T>(int number, WeightedSet<T> units, NextPosWeighting nextPos, NextUnitWeighting nextUnit, 
        Vector2Int dimensions, Encounter encounter, ref List<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        float UnitWeight(Unit u) => nextUnit(u, encounter, dimensions);
        float PosWeight(Vector2Int p) => nextPos(p, encounter, dimensions);
        for (int i = 0; i < number; ++ i)
        {
            var unitSet = new WeightedSet<T>(units);
            unitSet.ApplyMetric(UnitWeight);
            var unit = RandomU.instance.Choice(unitSet);
            var pos = RandomU.instance.Choice(validPositions, validPositions.Select(PosWeight));
            validPositions.Remove(pos);
            encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        }
    }

    private void PlaceUnitsRandom<T>(int number, WeightedSet<T> units, ref Encounter encounter, ref List<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        for (int i = 0; i < number; ++i)
        {
            var unit = RandomU.instance.Choice(units);
            var pos = RandomU.instance.Choice(validPositions);
            validPositions.Remove(pos);
            encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        }
    }

    private float ClumpWeight(Vector2Int pos, Encounter encounter, Vector2Int dimensions)
    {
        float weight = 1;
        foreach(var p in pos.Adjacent())
        {
            if(p.IsBoundedBy(dimensions) && encounter.units.FindIndex((e) => e.pos == p) != -1)
            {
                weight *= 2;
            }
        }
        return weight;
    }

    private float SpreadWeight(Vector2Int pos, Encounter encounter, Vector2Int dimensions)
    {
        float weight = 1;
        foreach (var p in pos.Adjacent())
        {
            if (p.IsBoundedBy(dimensions) && encounter.units.FindIndex((e) => e.pos == p) == -1)
            {
                weight *= 2;
            }
        }
        return weight;
    }

    private float PassThrough(Unit u, Encounter encounter, Vector2Int dimensions) => 0;
    //private float PassThrough(Vector2Int pos, Encounter encounter, Vector2Int dimensions) => 0;

    private void PlaceLoot(WeightedSet<MysteryDataUnit.Category> categories, WeightedSet<MysteryDataUnit.Quality> qualities, 
        ref Encounter encounter, ref List<Vector2Int> validPositions)
    {
        var category = RandomU.instance.Choice(categories);
        if (!lootUnitTable.ContainsKey(category))
            throw new System.Exception("Loot excpetion: category " + category.ToString() + " does not exist!");
        MysteryDataUnit.Quality quality = MysteryDataUnit.Quality.None;
        // Quality doesn't currently apply to boss, color, and gamble loot
        if (category != MysteryDataUnit.Category.Boss && category != MysteryDataUnit.Category.Gamble && category != MysteryDataUnit.Category.Color)
            quality = RandomU.instance.Choice(qualities);
        if (!lootUnitTable[category].ContainsKey(quality))
            throw new System.Exception("Loot excpetion: no data of quality " + quality.ToString() + "in category " + category.ToString());
        var unit = RandomU.instance.Choice(lootUnitTable[category][quality]);
        var pos = RandomU.instance.Choice(validPositions);
        encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        validPositions.Remove(pos);
    }

    private void SetSpawnPositions(int num, Vector2Int dimensions, Encounter encounter, ref List<Vector2Int> validPositions)
    {
        if(num > 0 && validPositions.Count > 0)
        {
            var weights = new WeightedSet<Vector2Int>(validPositions, (pos) => SpreadWeight(pos, encounter, dimensions));
            Vector2Int spawnPos = RandomU.instance.Choice(weights);
            encounter.spawnPositions.Add(spawnPos);
            validPositions.Remove(spawnPos);
        }
        for (int i = 1; i < num && validPositions.Count > 0; ++i)
        {
            var weights = new WeightedSet<Vector2Int>(validPositions, (pos) => ClumpWeight(pos,encounter, dimensions));//(pos) => SpreadWeight(pos, encounter, dimensions));
            Vector2Int spawnPos = RandomU.instance.Choice(weights);
            encounter.spawnPositions.Add(spawnPos);
            validPositions.Remove(spawnPos);
        }
    }
}
