using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

public class LootManager : MonoBehaviour
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
    }

    public List<Program> lootPrograms = new List<Program>();

    private Dictionary<Rarity, List<Program>> dropTables = new Dictionary<Rarity, List<Program>>();

    private Dictionary<LootQuality, WeightedSet<Rarity>> standardLootRarities;

    private void Awake()
    {
        BuildDropTables();
        BuildStandardLootRarities();
    }

    private void BuildDropTables()
    {
        dropTables.Clear();
        foreach (var program in lootPrograms)
        {
            if (!dropTables.ContainsKey(program.Rarity))
                dropTables.Add(program.Rarity, new List<Program>());
            dropTables[program.Rarity].Add(program);
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
        standardLootRarities = new Dictionary<LootQuality, WeightedSet<Rarity>>()
        {
            { LootQuality.Standard, standardWeights },
            { LootQuality.High, highWeights },
            { LootQuality.Max, maxWeights },
            { LootQuality.Even, evenWeights },
            { LootQuality.Boss, bossWeights },
        };
    }

    public Program GetDropStandard(LootQuality quality, System.Predicate<Program> filter = null)
    {
        return GetDropCustom(standardLootRarities[quality], filter);
    }

    public Program GetDropCustom(WeightedSet<Rarity> rarities, System.Predicate<Program> filter = null)
    {
        var rarity = RandomU.instance.Choice(rarities);
        if(filter == null)
            return RandomU.instance.Choice(dropTables[rarity]);
        return RandomU.instance.Choice(dropTables[rarity].Where((p) => filter(p)));
    }
}
