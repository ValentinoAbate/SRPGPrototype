using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterNew", menuName = "Event Data / Encounter Data")]
public class EncounterData : ScriptableObject
{
    public Vector2Int dimensions = new Vector2Int(8, 8);
    public int budget = 5;
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public List<ObstacleUnit> obstacles = new List<ObstacleUnit>();
    public Encounter seed = null;
}
