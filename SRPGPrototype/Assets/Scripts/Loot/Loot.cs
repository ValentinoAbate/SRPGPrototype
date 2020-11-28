using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

public class Loot<T> where T : ILootable
{
    public const int standardDraws = 3;
    /// <summary>
    /// A link to the 
    /// </summary>
    public enum LootQuality
    {
        Standard,
        High,
        Max,
        Boss,
        Even,
    }

    private readonly Dictionary<Rarity, WeightedSet<T>> dropTables = new Dictionary<Rarity, WeightedSet<T>>();

    private Dictionary<LootQuality, WeightedSet<Rarity>> standardLootRarities;

    public Loot(List<T> loot)
    {
        BuildStandardLootRarities();
        BuildDropTables(loot);
    }

    private void BuildDropTables(List<T> loot)
    {
        dropTables.Clear();
        foreach (var item in loot)
        {
            if (!dropTables.ContainsKey(item.Rarity))
                dropTables.Add(item.Rarity, new WeightedSet<T>());
            dropTables[item.Rarity].Add(item, item.LootWeight);
        }
    }

    private void BuildStandardLootRarities()
    {
        var standardWeights = new WeightedSet<Rarity>
        {
            {Rarity.Common, 75 },
            {Rarity.Uncommon, 22 },
            {Rarity.Rare, 3 },
        };
        var highWeights = new WeightedSet<Rarity>
        {
            {Rarity.Common, 15},
            {Rarity.Uncommon, 75 },
            {Rarity.Rare, 10 },
        };
        var maxWeights = new WeightedSet<Rarity>
        {
            {Rarity.Uncommon, 25 },
            {Rarity.Rare, 75 },
        };
        var evenWeights = new WeightedSet<Rarity>
        {
            {Rarity.Common, 1},
            {Rarity.Uncommon, 1 },
            {Rarity.Rare, 1 },
        };
        var bossWeights = new WeightedSet<Rarity>
        {
            {Rarity.Boss, 1 },
        };
        standardLootRarities = new Dictionary<LootQuality, WeightedSet<Rarity>>()
        {
            { LootQuality.Standard, standardWeights },
            { LootQuality.High, highWeights },
            { LootQuality.Max, maxWeights },
            { LootQuality.Even, evenWeights },
            { LootQuality.Boss, bossWeights },
        };
    }

    public T GetDropStandard(LootQuality quality, System.Predicate<T> filter = null)
    {
        return GetDropCustom(standardLootRarities[quality], filter);
    }

    public List<T> GetDropsStandard(LootQuality quality, int drops = standardDraws, System.Predicate<T> filter = null)
    {
        return GetDropsStandard(Enumerable.Repeat(quality, drops), filter);
    }

    public List<T> GetDropsStandard(IEnumerable<LootQuality> qualities, System.Predicate<T> filter = null)
    {
        var ret = new List<T>(qualities.Count());
        foreach (var quality in qualities)
        {
            ret.Add(GetDropCustom(standardLootRarities[quality], filter));
        }
        return ret;
    }

    public List<T> GetDropsStandardNoDuplicates(LootQuality quality, int drops = standardDraws, System.Predicate<T> filter = null)
    {
        return GetDropsStandardNoDuplicates(Enumerable.Repeat(quality, drops), filter);
    }

    public List<T> GetDropsStandardNoDuplicates(IEnumerable<LootQuality> qualities, System.Predicate<T> filter = null)
    { 
        var ret = new List<T>(qualities.Count());
        // Local filter function to call input filter and  assure no duplicates
        bool NoDupeFilter(T item)
        {
            if (filter != null && !filter(item))
                return false;
            return !ret.Contains(item);
        }
        foreach (var quality in qualities)
        {
            ret.Add(GetDropCustom(standardLootRarities[quality], NoDupeFilter));
        }
        return ret;
    }

    public T GetDropCustom(WeightedSet<Rarity> rarities, System.Predicate<T> filter = null)
    {
        var rarity = RandomU.instance.Choice(rarities);
        if(filter == null)
            return RandomU.instance.Choice(dropTables[rarity]);
        return RandomU.instance.Choice(new WeightedSet<T>(dropTables[rarity].Where((p) => filter(p.Key))));
    }
}
