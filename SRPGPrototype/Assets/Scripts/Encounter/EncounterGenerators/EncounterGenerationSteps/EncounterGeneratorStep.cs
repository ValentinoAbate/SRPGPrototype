using Extensions.VectorIntDimensionUtils;
using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EncounterGenerator;

public abstract class EncounterGeneratorStep : ScriptableObject
{
    public delegate float NextPosWeighting(Vector2Int pos, Encounter encounter);

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

    public static void AddUnit(Unit unit, Vector2Int pos, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        validPositions.Remove(pos);
        encounter.units.Add(new Encounter.UnitEntry(unit, pos));
    }

    public static void PlaceUnitsWeighted<T>(int number, WeightedSet<T> units, NextPosWeighting nextPos, Encounter encounter, ref HashSet<Vector2Int> validPositions, System.Action<WeightedSet<T>, T> unitSetAdjustmentFn = null) where T : Unit
    {
        var unitSet = new WeightedSet<T>(units);
        float PosWeight(Vector2Int p) => nextPos(p, encounter);
        for (int i = 0; i < number; ++i)
        {
            var unit = RandomU.instance.Choice(unitSet);
            var pos = RandomU.instance.Choice(new WeightedSet<Vector2Int>(validPositions, PosWeight));
            validPositions.Remove(pos);
            encounter.units.Add(new Encounter.UnitEntry(unit, pos));
            if(unitSetAdjustmentFn != null)
            {
                unitSetAdjustmentFn.Invoke(unitSet, unit);
                if (unitSet.Count <= 0)
                    return;
            }
        }
    }

    public static void PlaceUnitsRandom<T>(int number, WeightedSet<T> units, ref Encounter encounter, ref HashSet<Vector2Int> validPositions) where T : Unit
    {
        for (int i = 0; i < number; ++i)
        {
            var unit = RandomU.instance.Choice(units);
            var pos = RandomU.instance.Choice(validPositions);
            validPositions.Remove(pos);
            encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        }
    }

    public static float ClumpWeight(Vector2Int pos, Encounter encounter)
    {
        float weight = 1;
        foreach (var p in pos.Adjacent())
        {
            if (p.IsBoundedBy(encounter.dimensions) && encounter.units.FindIndex((e) => e.pos == p) != -1)
            {
                weight *= 2;
            }
        }
        return weight;
    }

    public static float SpreadWeight(Vector2Int pos, Encounter encounter)
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

    public static void GetLootWeights(EncounterDifficulty difficulty, out WeightedSet<MysteryDataUnit.Category> categoryWeights, out WeightedSet<MysteryDataUnit.Quality> qualityWeights)
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

    public static void PlaceLootDefault(LootModifiers lootFlags, WeightedSet<MysteryDataUnit.Category> categoryWeights, WeightedSet<MysteryDataUnit.Quality> qualityWeights, LootUnitSet lootUnits, ref Encounter encounter, ref HashSet<Vector2Int> positions)
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

    public static void PlaceLoot(WeightedSet<MysteryDataUnit.Category> categories, WeightedSet<MysteryDataUnit.Quality> qualities, LootUnitSet lootUnits, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
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

    public static void ChooseSpawnPositionsDefault(int num, Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        if (num > 0 && validPositions.Count > 0)
        {
            ChooseSpawnPositionsWeighted(1, (pos) => SpreadWeight(pos, encounter), ref encounter, ref validPositions);
        }
        ChooseSpawnPositionsWeighted(num - 1, (pos) => ClumpWeight(pos, encounter), ref encounter, ref validPositions);
    }

    protected static void ChooseSpawnPositionsWeighted(int number, System.Func<Vector2Int, float> weightFn, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        for (int i = 0; i < number && validPositions.Count > 0; ++i)
        {
            Vector2Int spawnPos = RandomU.instance.Choice(new WeightedSet<Vector2Int>(validPositions, weightFn));
            encounter.spawnPositions.Add(spawnPos);
            validPositions.Remove(spawnPos);
        }
    }

    protected static void ChooseSpawnPositionsWeightedEnc(int number, System.Func<Vector2Int, Encounter, float> weightFn, Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        float Weight(Vector2Int pos) => weightFn(pos, encounter);
        for (int i = 0; i < number && validPositions.Count > 0; ++i)
        {
            Vector2Int spawnPos = RandomU.instance.Choice(new WeightedSet<Vector2Int>(validPositions, Weight));
            encounter.spawnPositions.Add(spawnPos);
            validPositions.Remove(spawnPos);
        }
    }

    public abstract void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions);

    public class Metadata
    {
        public float targetDifficulty;
    }
}
