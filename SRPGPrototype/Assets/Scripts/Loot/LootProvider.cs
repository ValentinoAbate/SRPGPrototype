using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LootProvider
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

    protected static readonly Dictionary<LootQuality, WeightedSet<Rarity>> standardLootRarities;

    static LootProvider()
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
}
