using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the data from the maps, including the current encounter
/// </summary>
public class MapManager : MonoBehaviour
{
    public Map Map => maps[currMap];
    public Encounter Encounter => Map.Current.value;

    [SerializeField] private MapData[] data = null;
    [SerializeField] private MapGenerator generator = null;
    private List<Map> maps = new List<Map>();
    private int currMap = 0;

    private void Start()
    {
        foreach(var d in data)
        {
            maps.Add(generator.Generate(d));
        }
    }
}
