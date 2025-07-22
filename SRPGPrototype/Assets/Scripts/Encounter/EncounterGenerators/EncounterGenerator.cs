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
    public enum EncounterDifficulty
    {
        Normal,
        Easy,
        Hard
    }

    public delegate float NextPosWeighting(Vector2Int pos, Encounter encounter);
    public delegate float NextUnitWeighting(Unit u, Encounter encounter);

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

    public static void PlaceUnitsWeighted<T>(int number, WeightedSet<T> units, NextPosWeighting nextPos, NextUnitWeighting nextUnit,
        Encounter encounter, ref HashSet<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        float UnitWeight(Unit u) => nextUnit(u, encounter);
        float PosWeight(Vector2Int p) => nextPos(p, encounter);
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

    protected static float ClumpWeight(Vector2Int pos, Encounter encounter)
    {
        float weight = 1;
        foreach(var p in pos.Adjacent())
        {
            if(p.IsBoundedBy(encounter.dimensions) && encounter.units.FindIndex((e) => e.pos == p) != -1)
            {
                weight *= 2;
            }
        }
        return weight;
    }

    protected static float SpreadWeight(Vector2Int pos, Encounter encounter)
    {
        float weight = 1;
        foreach (var p in pos.Adjacent())
        {
            if (p.IsBoundedBy(encounter.dimensions) && encounter.units.FindIndex((e) => e.pos == p) == -1)
            {
                weight *= 2;
            }
        }
        return weight;
    }

    protected static float PassThrough(Unit _, Encounter _2) => 0;

    public static void GetLootWeights(EncounterDifficulty difficulty, out WeightedSet<MysteryDataUnit.Category> categoryWeights,
        out WeightedSet<MysteryDataUnit.Quality> qualityWeights)
    {
        if (difficulty == EncounterDifficulty.Easy)
        {
            categoryWeights = lootCategoryWeightsEasy;
            qualityWeights = lootQualityWeightsEasy;
        }
        else if (difficulty == EncounterDifficulty.Hard)
        {
            categoryWeights = lootCategoryWeightsDifficult;
            qualityWeights = lootQualityWeightsDifficult;
        }
        else
        {
            categoryWeights = lootCategoryWeights;
            qualityWeights = lootQualityWeights;
        }
    }

    protected void PlaceLootDefault(LootModifiers lootFlags, WeightedSet<MysteryDataUnit.Category> categoryWeights, WeightedSet<MysteryDataUnit.Quality> qualityWeights,
        ref Encounter encounter, ref HashSet<Vector2Int> positions)
    {
        PlaceLootDefault(lootFlags, categoryWeights, qualityWeights, lootUnits, ref encounter, ref positions);
    }

    public static void PlaceLootDefault(LootModifiers lootFlags, WeightedSet<MysteryDataUnit.Category> categoryWeights, WeightedSet<MysteryDataUnit.Quality> qualityWeights,
        LootUnitSet lootUnits, ref Encounter encounter, ref HashSet<Vector2Int> positions)
    {
        // Place the default loot unless there shouldn't be normal loot 
        if (!lootFlags.HasFlag(LootModifiers.NoNormalLoot))
        {
            // Actually place the loot
            PlaceLoot(categoryWeights, qualityWeights, lootUnits, ref encounter, ref positions);
        }
        // Place the bonus default loot (if applicable)
        if (lootFlags.HasFlag(LootModifiers.Bonus))
        {
            PlaceLoot(categoryWeights, qualityWeights, lootUnits, ref encounter, ref positions);
        }
        // Place the bonus money loot (if applicable)
        if (lootFlags.HasFlag(LootModifiers.BonusMoney))
        {
            PlaceLoot(moneyLootCategoryWeights, qualityWeights, lootUnits, ref encounter, ref positions);
        }
        // Place the bonus money or default loot (if applicable)
        if (lootFlags.HasFlag(LootModifiers.BonusRandom))
        {
            var hybridWeights = new WeightedSet<MysteryDataUnit.Category>(categoryWeights);
            hybridWeights.Add(moneyLootCategoryWeights);
            PlaceLoot(hybridWeights, qualityWeights, lootUnits, ref encounter, ref positions);
        }
        // Place the shell loot (if applicable)
        if (lootFlags.HasFlag(LootModifiers.Shell))
        {
            PlaceLoot(midbossLootCategoryWeights, qualityWeights, lootUnits, ref encounter, ref positions);
        }
        // Place the boss capacity loot (if applicable)
        if (lootFlags.HasFlag(LootModifiers.BossCapacity))
        {
            PlaceLoot(bossLootCategoryWeights, qualityWeights, lootUnits, ref encounter, ref positions);
        }
    }

    public static void PlaceLoot(WeightedSet<MysteryDataUnit.Category> categories, WeightedSet<MysteryDataUnit.Quality> qualities, 
        LootUnitSet lootUnits, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        var category = RandomU.instance.Choice(categories);
        if (!lootUnits.LootUnitTable.ContainsKey(category))
        {
            Debug.LogError("Loot Error: category " + category.ToString() + " does not exist!");
            return;
        }
        var quality = MysteryDataUnit.Quality.None;
        // Quality doesn't currently apply to boss, color, and gamble loot
        if (category != MysteryDataUnit.Category.Boss && category != MysteryDataUnit.Category.Gamble && category != MysteryDataUnit.Category.Color)
        {
            quality = RandomU.instance.Choice(qualities);
        }
        if (!lootUnits.LootUnitTable[category].ContainsKey(quality))
        {
            Debug.LogError("Loot Error: no data of quality " + quality.ToString() + "in category " + category.ToString());
            return;
        }
        var unit = RandomU.instance.Choice(lootUnits.LootUnitTable[category][quality]);
        var pos = RandomU.instance.Choice(validPositions);
        encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        validPositions.Remove(pos);
    }

    public static void SetSpawnPositions(int num, Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        if(num > 0 && validPositions.Count > 0)
        {
            var weights = new WeightedSet<Vector2Int>(validPositions, (pos) => SpreadWeight(pos, encounter));
            Vector2Int spawnPos = RandomU.instance.Choice(weights);
            encounter.spawnPositions.Add(spawnPos);
            validPositions.Remove(spawnPos);
        }
        for (int i = 1; i < num && validPositions.Count > 0; ++i)
        {
            var weights = new WeightedSet<Vector2Int>(validPositions, (pos) => ClumpWeight(pos, encounter));
            Vector2Int spawnPos = RandomU.instance.Choice(weights);
            encounter.spawnPositions.Add(spawnPos);
            validPositions.Remove(spawnPos);
        }
    }
}
