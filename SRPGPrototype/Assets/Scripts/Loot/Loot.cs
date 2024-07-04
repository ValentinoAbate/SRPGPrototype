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
        SoulCore,
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
        var soulCoreWeights = new WeightedSet<Rarity> 
        {
            {Rarity.SoulCore, 1} 
        };

        standardLootRarities = new Dictionary<LootQuality, WeightedSet<Rarity>>()
        {
            { LootQuality.Standard, standardWeights },
            { LootQuality.High, highWeights },
            { LootQuality.Max, maxWeights },
            { LootQuality.Even, evenWeights },
            { LootQuality.Boss, bossWeights },
            { LootQuality.SoulCore, soulCoreWeights }
        };
    }

    public T GetDropStandard(LootQuality quality, System.Predicate<T> filter = null)
    {
        return GetDropCustom(standardLootRarities[quality], filter);
    }

    public List<T> GetDropsStandard(LootQuality quality, int drops = standardDraws, System.Predicate<T> filter = null)
    {
        return GetDropsStandardInternal(Enumerable.Repeat(quality, drops), drops, filter);
    }

    public List<T> GetDropsStandard(ICollection<LootQuality> qualities, System.Predicate<T> filter = null)
    {
        return GetDropsStandardInternal(qualities, qualities.Count, filter);
    }

    private List<T> GetDropsStandardInternal(IEnumerable<LootQuality> qualities, int count, System.Predicate<T> filter)
    {
        var ret = new List<T>(count);
        foreach (var quality in qualities)
        {
            ret.Add(GetDropCustom(standardLootRarities[quality], filter));
        }
        return ret;
    }

    public List<T> GetDropsStandardNoDuplicates(LootQuality quality, int drops = standardDraws, System.Predicate<T> filter = null)
    {
        return GetDropsStandardNoDuplicates(Enumerable.Repeat(quality, drops).ToArray(), filter);
    }

    public List<T> GetDropsStandardNoDuplicates(IReadOnlyList<LootQuality> qualities, System.Predicate<T> filter = null)
    {
        T Generator(int index, System.Predicate<T> innerFilter)
        {
            return GetDropCustom(standardLootRarities[qualities[index]], innerFilter);
        }
        return GetDropsNoDuplicates(Generator, qualities.Count, filter);
    }

    public T GetDropCustom(WeightedSet<Rarity> rarities)
    {
        var rarity = RandomU.instance.Choice(rarities);
        return RandomU.instance.Choice(dropTables[rarity]);
    }

    public List<T> GetDropsCustomNoDuplicates(WeightedSet<Rarity> rarities, int drops)
    {
        return GetDropsCustomNoDuplicates(rarities, drops, null);
    }

    public T GetDropCustom(WeightedSet<Rarity> rarities, System.Predicate<T> filter)
    {
        if (filter == null)
            return GetDropCustom(rarities);
        var rarity = RandomU.instance.Choice(rarities);
        var dropTable = dropTables[rarity];
        var choices = new List<KeyValuePair<T, float>>(dropTable.Count);
        foreach(var kvp in dropTable)
        {
            if (filter(kvp.Key))
            {
                choices.Add(kvp);
            }
        }
        if (choices.Count <= 0)
            throw new System.Exception("No valid choices");
        return RandomU.instance.Choice<T>(choices);
    }

    public List<T> GetDropsCustomNoDuplicates(WeightedSet<Rarity> rarities, int drops, System.Predicate<T> filter)
    {
        T Generator(int index, System.Predicate<T> innerFilter)
        {
            return GetDropCustom(rarities, innerFilter);
        }
        return GetDropsNoDuplicates(Generator, drops, filter);
    }

    // Get a custom drop without any respect to rarity
    public T GetDropCustom(System.Predicate<T> filter)
    {
        var choices = new List<T>();
        foreach(var tableKvp in dropTables)
        {
            foreach(var kvp in tableKvp.Value)
            {
                if (filter(kvp.Key))
                {
                    choices.Add(kvp.Key);
                }
            }
        }
        return RandomU.instance.Choice(choices);
    }

    private T GetDropCustom(int index, System.Predicate<T> filter)
    {
        return GetDropCustom(filter);
    }

    public List<T> GetDropsCustomNoDuplicates(int drops, System.Predicate<T> filter)
    {
        return GetDropsNoDuplicates(GetDropCustom, drops, filter);
    }

    private delegate T DropFunction(int index, System.Predicate<T> filter);

    private List<T> GetDropsNoDuplicates(DropFunction generator, int count, System.Predicate<T> filter)
    {
        var ret = new List<T>(count);
        var lookup = new HashSet<T>();
        // Local filter function to call input filter and assure no duplicates
        bool NoDupeFilter(T item)
        {
            if (filter != null && !filter(item))
                return false;
            return !lookup.Contains(item);
        }
        for (int i = 0; i < count; ++i)
        {
            try
            {
                var drop = generator(i, NoDupeFilter);
                ret.Add(drop);
                lookup.Add(drop);
            }
            catch
            {

            }
        }
        if (ret.Count <= 0)
        {
            ret.Add(GetDropStandard(LootQuality.Standard));
            Debug.LogError("No applicable loot found");
        }
        return ret;
    }
}
