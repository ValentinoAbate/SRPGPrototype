using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterNew", menuName = "Event Data / Encounter Data")]
public class EncounterData : ScriptableObject
{
    public Vector2Int dimensions = new Vector2Int(8, 8);
    [Header("Enemies")]
    public int enemyBudget = 5;
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    [Header("Environment")]
    public int obstacleBudget = 0;
    public List<ObstacleUnit> obstacles = new List<ObstacleUnit>();
    [Header("Loot")]
    public int lootBudget = 2;
    public List<MysteryDataUnit> data = new List<MysteryDataUnit>();
    public Encounter seed = null;
}
