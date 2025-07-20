using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootUnitSet", menuName = "Encounter Generation/Loot Unit Set")]
public class LootUnitSet : ScriptableObject
{
    public IReadOnlyList<MysteryDataUnit> LootUnits => lootUnits;
    [SerializeField] private List<MysteryDataUnit> lootUnits;

    public IReadOnlyDictionary<MysteryDataUnit.Category, Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>> LootUnitTable
    {
        get
        {
            InitializeLootTable();
            return lootUnitTable;
        }
    }
    private readonly Dictionary<MysteryDataUnit.Category, Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>> lootUnitTable
    = new Dictionary<MysteryDataUnit.Category, Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>>();

    [NonSerialized] private bool lootTableInitialized = false;
    private void InitializeLootTable()
    {
        if (lootTableInitialized)
            return;
        lootTableInitialized = true;
        lootUnitTable.Clear();
        foreach (var unit in lootUnits)
        {
            if (!lootUnitTable.ContainsKey(unit.LootCategory))
                lootUnitTable.Add(unit.LootCategory, new Dictionary<MysteryDataUnit.Quality, List<MysteryDataUnit>>());
            if (!lootUnitTable[unit.LootCategory].ContainsKey(unit.LootQuality))
                lootUnitTable[unit.LootCategory].Add(unit.LootQuality, new List<MysteryDataUnit>() { unit });
            else
                lootUnitTable[unit.LootCategory][unit.LootQuality].Add(unit);
        }
    }
}
