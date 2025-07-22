using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions.VectorIntDimensionUtils;

public abstract class EncounterGenerator : ScriptableObject
{
    public enum Type
    {
        Encounter,
        Shop,
        Boss,
        Rest,
    }

    public delegate float NextPosWeighting(Vector2Int pos, Encounter encounter, Vector2Int dimensions);
    public delegate float NextUnitWeighting(Unit u, Encounter encounter, Vector2Int dimensions);

    #region Weightings

    protected static readonly WeightedSet<MysteryDataUnit.Category> lootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Standard, 72 },
        { MysteryDataUnit.Category.Color, 24 },
        { MysteryDataUnit.Category.Ability, 3 },
        { MysteryDataUnit.Category.Shell, 0.1f },
        { MysteryDataUnit.Category.Capacity, 0.1f },
        { MysteryDataUnit.Category.Gamble,  0.8f },
    };
    protected static readonly WeightedSet<MysteryDataUnit.Category> lootCategoryWeightsDifficult = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Standard, 50 },
        { MysteryDataUnit.Category.Color, 30 },
        { MysteryDataUnit.Category.Ability, 10 },
        { MysteryDataUnit.Category.Shell, 1 },
        { MysteryDataUnit.Category.Capacity, 1 },
        { MysteryDataUnit.Category.Gamble,  8 },
    };
    protected static readonly WeightedSet<MysteryDataUnit.Category> lootCategoryWeightsEasy = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Standard, 85 },
        { MysteryDataUnit.Category.Color, 14 },
        { MysteryDataUnit.Category.Gamble,  1 },
    };
    protected static readonly WeightedSet<MysteryDataUnit.Quality> lootQualityWeights = new WeightedSet<MysteryDataUnit.Quality>
    {
        { MysteryDataUnit.Quality.Common, 89 },
        { MysteryDataUnit.Quality.Uncommon, 10 },
        { MysteryDataUnit.Quality.Rare, 1 },
    };
    protected static readonly WeightedSet<MysteryDataUnit.Quality> lootQualityWeightsDifficult = new WeightedSet<MysteryDataUnit.Quality>
    {
        { MysteryDataUnit.Quality.Common, 60 },
        { MysteryDataUnit.Quality.Uncommon, 35 },
        { MysteryDataUnit.Quality.Rare, 5 },
    };
    protected static readonly WeightedSet<MysteryDataUnit.Quality> lootQualityWeightsEasy = new WeightedSet<MysteryDataUnit.Quality>
    {
        { MysteryDataUnit.Quality.Common, 95 },
        { MysteryDataUnit.Quality.Uncommon, 4.75f },
        { MysteryDataUnit.Quality.Rare, 0.25f },
    };
    protected static readonly WeightedSet<MysteryDataUnit.Category> midbossLootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Shell, 100 }
    };
    protected static readonly WeightedSet<MysteryDataUnit.Category> bossLootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Boss, 100 }
    };
    protected static readonly WeightedSet<MysteryDataUnit.Category> moneyLootCategoryWeights = new WeightedSet<MysteryDataUnit.Category>
    {
        { MysteryDataUnit.Category.Money, 100 },
    };

    #endregion


    [Header("General")]
    [SerializeField] private Type encounterType;
    [SerializeField] private Vector2Int dimensions = new Vector2Int(8, 8);
    [SerializeField] private LootUnitSet lootUnits;

    public abstract Encounter Generate(string mapSymbol, int encounterNumber);

    protected void InitializeEncounter(string mapSymbol, int encounterNumber, out HashSet<Vector2Int> positions, out Encounter encounter)
    {
        positions = new HashSet<Vector2Int>(dimensions.Enumerate());
        encounter = new Encounter() { nameOverride = $"{encounterType} {mapSymbol}-{encounterNumber}", dimensions = dimensions };
    }

    protected void ApplySeed(Encounter seed, ref Encounter encounter, ref HashSet<Vector2Int> positions)
    {
        // Initialize encounter values from seed if applicable
        if (seed != null)
        {
            var outOfBoundsUnits = new List<Encounter.UnitEntry>(seed.units.Count);
            // Place seed units
            foreach (var entry in seed.units)
            {
                // Position is invalid, generate a proper position later
                if (!positions.Contains(entry.pos))
                {
                    outOfBoundsUnits.Add(entry);
                    continue;
                }
                encounter.units.Add(entry);
                positions.Remove(entry.pos);
            }
            // Process seed spawn positions
            foreach(var spawnPosition in seed.spawnPositions)
            {
                positions.Remove(spawnPosition);
                encounter.spawnPositions.Add(spawnPosition);
            }
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
    }

    protected void PlaceUnitsWeighted<T>(int number, WeightedSet<T> units, NextPosWeighting nextPos, NextUnitWeighting nextUnit,
        Encounter encounter, ref HashSet<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        PlaceUnitsWeighted(number, units, nextPos, nextUnit, dimensions, encounter, ref validPositions);
    }
    public static void PlaceUnitsWeighted<T>(int number, WeightedSet<T> units, NextPosWeighting nextPos, NextUnitWeighting nextUnit, 
        Vector2Int dimensions, Encounter encounter, ref HashSet<Vector2Int> validPositions) where T : Unit, IEncounterUnit
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

    public static void PlaceUnitsRandom<T>(int number, WeightedSet<T> units, ref Encounter encounter, ref HashSet<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        for (int i = 0; i < number; ++i)
        {
            var unit = RandomU.instance.Choice(units);
            var pos = RandomU.instance.Choice(validPositions);
            validPositions.Remove(pos);
            encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        }
    }

    protected static float ClumpWeight(Vector2Int pos, Encounter encounter, Vector2Int dimensions)
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

    protected static float SpreadWeight(Vector2Int pos, Encounter encounter, Vector2Int dimensions)
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

    protected static float PassThrough(Unit _, Encounter _2, Vector2Int _3) => 0;
    //private float PassThrough(Vector2Int pos, Encounter encounter, Vector2Int dimensions) => 0;

    protected void PlaceLoot(WeightedSet<MysteryDataUnit.Category> categories, WeightedSet<MysteryDataUnit.Quality> qualities, 
        ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        PlaceLoot(categories, qualities, lootUnits, ref encounter, ref validPositions);
    }

    public static void PlaceLoot(WeightedSet<MysteryDataUnit.Category> categories, WeightedSet<MysteryDataUnit.Quality> qualities, 
        LootUnitSet lootUnits, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        var category = RandomU.instance.Choice(categories);
        if (!lootUnits.LootUnitTable.ContainsKey(category))
            throw new System.Exception("Loot excpetion: category " + category.ToString() + " does not exist!");
        MysteryDataUnit.Quality quality = MysteryDataUnit.Quality.None;
        // Quality doesn't currently apply to boss, color, and gamble loot
        if (category != MysteryDataUnit.Category.Boss && category != MysteryDataUnit.Category.Gamble && category != MysteryDataUnit.Category.Color)
            quality = RandomU.instance.Choice(qualities);
        if (!lootUnits.LootUnitTable[category].ContainsKey(quality))
            throw new System.Exception("Loot excpetion: no data of quality " + quality.ToString() + "in category " + category.ToString());
        var unit = RandomU.instance.Choice(lootUnits.LootUnitTable[category][quality]);
        var pos = RandomU.instance.Choice(validPositions);
        encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        validPositions.Remove(pos);
    }

    protected void SetSpawnPositions(int num, Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        SetSpawnPositions(num, dimensions, encounter, ref validPositions);
    }

    public static void SetSpawnPositions(int num, Vector2Int dimensions, Encounter encounter, ref HashSet<Vector2Int> validPositions)
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
