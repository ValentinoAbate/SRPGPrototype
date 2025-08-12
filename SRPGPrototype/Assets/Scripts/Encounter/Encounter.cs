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
