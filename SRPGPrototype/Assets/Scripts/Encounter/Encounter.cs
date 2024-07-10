using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter
{
    public string name = "Encounter";
    public Vector2Int dimensions;
    public List<UnitEntry> units = new List<UnitEntry>();
    public List<Vector2Int> spawnPositions = new List<Vector2Int>();
    public bool giveCompletionMoney;
    public int baseCompletionMoney;
    public int completionMoneyVariance;
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
