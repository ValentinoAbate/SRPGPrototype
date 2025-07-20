using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions.VectorIntDimensionUtils;

public abstract class EncounterGenerator : ScriptableObject
{
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



    [SerializeField] private LootUnitSet lootUnits;

    public abstract Encounter Generate(string mapSymbol, int encounterNumber);

    protected void PlaceUnitsWeighted<T>(int number, WeightedSet<T> units, NextPosWeighting nextPos, NextUnitWeighting nextUnit, 
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

    protected void PlaceUnitsRandom<T>(int number, WeightedSet<T> units, ref Encounter encounter, ref List<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        for (int i = 0; i < number; ++i)
        {
            var unit = RandomU.instance.Choice(units);
            var pos = RandomU.instance.Choice(validPositions);
            validPositions.Remove(pos);
            encounter.units.Add(new Encounter.UnitEntry(unit, pos));
        }
    }

    protected float ClumpWeight(Vector2Int pos, Encounter encounter, Vector2Int dimensions)
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

    protected float SpreadWeight(Vector2Int pos, Encounter encounter, Vector2Int dimensions)
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

    protected float PassThrough(Unit u, Encounter encounter, Vector2Int dimensions) => 0;
    //private float PassThrough(Vector2Int pos, Encounter encounter, Vector2Int dimensions) => 0;

    protected void PlaceLoot(WeightedSet<MysteryDataUnit.Category> categories, WeightedSet<MysteryDataUnit.Quality> qualities, 
        ref Encounter encounter, ref List<Vector2Int> validPositions)
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

    protected void SetSpawnPositions(int num, Vector2Int dimensions, Encounter encounter, ref List<Vector2Int> validPositions)
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
