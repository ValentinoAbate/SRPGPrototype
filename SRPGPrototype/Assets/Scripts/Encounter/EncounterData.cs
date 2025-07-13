using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterNew", menuName = "Event Data / Encounter Data")]
public class EncounterData : ScriptableObject
{
    [System.Flags]
    public enum LootModifiers
    { 
        None = 0,
        Shell = 1,
        BossCapacity = 2,
        Bonus = 4,
        NoNormalLoot = 8,
        BonusMoney = 16,
        BonusRandom = 32,
    }
    public enum MoneyOption
    {
        None,
        Normal,
        Miniboss,
        Boss,
    }
    public enum Type 
    {
        Encounter,
        Shop,
        Boss,
        Rest,
    }

    [Header("General")]
    public float targetDifficulty;
    public Vector2Int dimensions = new Vector2Int(8, 8);
    public int numSpawnPositions = 3;
    public Type encounterType;
    [Header("Enemies")]
    public List<int> numEnemies = new List<int>{ 5 };
    public List<float> numEnemiesWeights = new List<float>{ 1 };
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public List<float> baseEnemyWeights = new List<float>() { 1 };
    [Header("Environment")]
    public List<int> numObstacles = new List<int> { 3 };
    public List<float> numObstaclesWeights = new List<float> { 1 };
    public List<ObstacleUnit> obstacles = new List<ObstacleUnit>();
    public List<float> baseObstacleWeights = new List<float>() { 1 };
    [Header("Loot")]
    public LootModifiers lootFlags;
    public MoneyOption moneyOption;
    [Header("Seed Properties")]
    public Encounter seed = null;
}
