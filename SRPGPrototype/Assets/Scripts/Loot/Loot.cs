using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

public class Loot<T> where T : ILootable
{
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
        Elite,
    }

    private readonly Dictionary<Rarity, List<T>> dropTables = new Dictionary<Rarity, List<T>>();

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
                dropTables.Add(item.Rarity, new List<T>());
            dropTables[item.Rarity].Add(item);
        }
    }

    private void BuildStandardLootRarities()
    {
        var standardWeights = new WeightedSet<Rarity>
        {
            {Rarity.Common, 2},
            {Rarity.Uncommon, 0.5f },
            {Rarity.Rare, 0.125f },
        };
        var highWeights = new WeightedSet<Rarity>
        {
            {Rarity.Common, 0.5f},
            {Rarity.Uncommon, 2 },
            {Rarity.Rare, 0.25f },
        };
        var maxWeights = new WeightedSet<Rarity>
        {
            {Rarity.Common, 0.25f},
            {Rarity.Uncommon, 1 },
            {Rarity.Rare, 2 },
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
        var eliteWeights = new WeightedSet<Rarity>
        {
            {Rarity.Elite, 1 },
        };
        standardLootRarities = new Dictionary<LootQuality, WeightedSet<Rarity>>()
        {
            { LootQuality.Standard, standardWeights },
            { LootQuality.High, highWeights },
            { LootQuality.Max, maxWeights },
            { LootQuality.Even, evenWeights },
            { LootQuality.Boss, bossWeights },
            { LootQuality.Elite, eliteWeights },
        };
    }

    public T GetDropStandard(LootQuality quality, System.Predicate<T> filter = null)
    {
        return GetDropCustom(standardLootRarities[quality], filter);
    }

    public List<T> GetDropsStandard(int number, LootQuality quality, System.Predicate<T> filter = null)
    {
        var ret = new List<T>(number);
        for (int i = 0; i < number; ++i)
            ret.Add(GetDropCustom(standardLootRarities[quality], filter));
        return ret;
    }

    public T GetDropCustom(WeightedSet<Rarity> rarities, System.Predicate<T> filter = null)
    {
        var rarity = RandomU.instance.Choice(rarities);
        if(filter == null)
            return RandomU.instance.Choice(dropTables[rarity]);
        return RandomU.instance.Choice(dropTables[rarity].Where((p) => filter(p)));
    }
}
