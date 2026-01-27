using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter
{
    public float Difficulty
    {
        get
        {
            float difficulty = 0;
            foreach (var unit in units)
            {
                if (!(unit.unit is IEncounterUnit encounterUnit))
                    continue;
                difficulty += encounterUnit.EncounterData.challengeRating;
            }
            foreach (var unit in ambushUnits)
            {
                if (!(unit.unit is IEncounterUnit encounterUnit))
                    continue;
                difficulty += encounterUnit.EncounterData.challengeRating;
            }
            return difficulty;
        }
    }

    public string nameOverride = string.Empty;
    public Vector2Int dimensions;
    public IReadOnlyList<UnitEntry> Units => units;
    [SerializeField] private List<UnitEntry> units = new List<UnitEntry>();
    public List<UnitEntry> ambushUnits = new List<UnitEntry>();
    public List<Vector2Int> spawnPositions = new List<Vector2Int>();
    public bool giveCompletionMoney;
    public int baseCompletionMoney;
    public int completionMoneyVariance;

    private Dictionary<Vector2Int, Unit> unitDict = new Dictionary<Vector2Int, Unit>();

    public void AddUnit(Unit unit, Vector2Int pos)
    {
        AddUnit(new UnitEntry(unit, pos));
    }
    public void AddUnit(UnitEntry entry)
    {
        units.Add(entry);
        unitDict.Add(entry.pos, entry.unit);
    }
    public bool HasUnitAt(Vector2Int pos) => unitDict.ContainsKey(pos);
    public bool TryGetUnit(Vector2Int pos, out Unit u)
    {
        return unitDict.TryGetValue(pos, out u);
    }

    public LootUI.MoneyData CompletionMoneyData()
    {
        int money = baseCompletionMoney;
        if(completionMoneyVariance != 0)
        {
            money += RandomUtils.RandomU.instance.RandomInt(-completionMoneyVariance, completionMoneyVariance + 1);
        }
        return new LootUI.MoneyData(money);
    }

    public SaveManager.EncounterData Save()
    {
        var savedEnc = new SaveManager.EncounterData
        {
            name = nameOverride,
            dim = dimensions,
            units = new List<SaveManager.GridPrefab>(Units.Count),
            ambush = new List<SaveManager.GridPrefab>(ambushUnits.Count),
            spawns = new List<Vector2Int>(spawnPositions),
            money = giveCompletionMoney,
            moneyBase = baseCompletionMoney,
            moneyVar = completionMoneyVariance,
        };
        foreach (var unit in units)
        {
            savedEnc.units.Add(SaveUnitEntry(unit));
        }
        foreach (var unit in ambushUnits)
        {
            savedEnc.ambush.Add(SaveUnitEntry(unit));
        }
        return savedEnc;
    }

    private SaveManager.GridPrefab SaveUnitEntry(UnitEntry entry)
    {
        return new SaveManager.GridPrefab()
        {
            key = entry.unit.Key,
            pos = entry.pos,
        };
    }

    public void Load(SaveManager.EncounterData data)
    {
        nameOverride = data.name;
        dimensions = data.dim;
        LoadUnitEntries(data.units, units);
        LoadUnitEntries(data.ambush, ambushUnits);
        spawnPositions.Clear();
        spawnPositions.AddRange(data.spawns);
        giveCompletionMoney = data.money;
        baseCompletionMoney = data.moneyBase;
        completionMoneyVariance = data.moneyVar;
    }

    public void LoadUnitEntries(List<SaveManager.GridPrefab> data, List<UnitEntry> container)
    {
        container.Clear();
        container.EnsureCapacity(data.Count);
        foreach (var unitData in data)
        {
            if (Lookup.TryGetUnit(unitData.key, out var unit))
            {
                container.Add(new UnitEntry(unit, unitData.pos));
            }
        }
    }

    [System.Serializable]
    public struct UnitEntry
    {
        public Vector2Int pos;
        public Unit unit;
        public UnitEntry(Unit unit, Vector2Int pos)
        {
            this.unit = unit;
            this.pos = pos;
        }
    }
}
