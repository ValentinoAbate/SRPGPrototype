using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncounterGenerator : MonoBehaviour
{
    private const float difficultyScale = -100;
    public delegate float NextPosWeighting(Vector2Int pos, Encounter encounter, Vector2Int dimensions);
    public delegate float NextUnitWeighting(Unit u, Encounter encounter, Vector2Int dimensions);

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
    private readonly WeightedSet<MysteryDataUnit.Quality> bossLootQualityWeights = new WeightedSet<MysteryDataUnit.Quality>
    {
        { MysteryDataUnit.Quality.None, 100 }
    };

    private readonly Dictionary<MysteryDataUnit.Category, Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>> lootUnitTable 
        = new Dictionary<MysteryDataUnit.Category, Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>>();

    [SerializeField] private List<MysteryDataUnit> lootUnits = new List<MysteryDataUnit>();

    private void Awake()
    {
        BuildLootUnitTable();
    }

    private void BuildLootUnitTable()
    {
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

    public Encounter Generate(EncounterData data, string encounterName)
    {
        var positions = EnumerateDimensions(data.dimensions);
        var encounter = new Encounter() { name = encounterName };
        int numEnemies = RandomU.instance.Choice(data.numEnemies, data.numEnemiesWeights);
        int numObstacles = RandomU.instance.Choice(data.numObstacles, data.numObstaclesWeights);
        var enemies = new WeightedSet<EnemyUnit>(data.enemies, data.baseEnemyWeights);
        var obstacles = new WeightedSet<ObstacleUnit>(data.obstacles, data.baseObstacleWeights);
        var lootFlags = data.lootFlags;
        //var lootUnits = new WeightedSet<MysteryDataUnit>(data.data, 1);
        // Initialize encounter values from seed if applicable
        if (data.seed != null)
        {
            encounter.units.AddRange(data.seed.units);
            encounter.reinforcements.AddRange(data.seed.reinforcements);
            positions = positions.Where((pos) => data.seed.units.All((unit) => unit.pos != pos)).ToList();
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
        float difficultyMod = difficulty - data.targetDifficulty;

        // Generate Loot category weights
        // Start with standard loot category weights
        var categoryWeights = new WeightedSet<MysteryDataUnit.Category>();
        var qualityWeights = new WeightedSet<MysteryDataUnit.Quality>();
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


        PlaceLoot(categoryWeights, qualityWeights, ref encounter, ref positions);

        if(lootFlags.HasFlag(EncounterData.LootModifiers.Bonus))
        {
            PlaceLoot(categoryWeights, qualityWeights, ref encounter, ref positions);
        }
        if(lootFlags.HasFlag(EncounterData.LootModifiers.Midboss))
        {
            PlaceLoot(midbossLootCategoryWeights, qualityWeights, ref encounter, ref positions);
        }
        if(lootFlags.HasFlag(EncounterData.LootModifiers.Boss))
        {
            PlaceLoot(bossLootCategoryWeights, qualityWeights, ref encounter, ref positions);
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
            if(p.x > 0 && p.y > 0 && p.x < dimensions.x && p.y < dimensions.y 
                && encounter.units.FindIndex((e) => e.pos == p) != -1)
            {
                weight *= 2;
            }
        }
        return weight;
    }

    private float PassThrough(Unit u, Encounter encounter, Vector2Int dimensions) => 0;
    private float PassThrough(Vector2Int pos, Encounter encounter, Vector2Int dimensions) => 0;

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
    }

    private List<Vector2Int> EnumerateDimensions(Vector2Int dimensions)
    {
        var ret = new List<Vector2Int>();
        for (int x = 0; x < dimensions.x; ++x)
            for (int y = 0; y < dimensions.y; ++y)
                ret.Add(new Vector2Int(x, y));
        return ret;
    }
}
