using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the data from the maps, including the current encounter
/// </summary>
public class MapManager : MonoBehaviour
{
    public Map Map { get; private set; }
    public bool SkipMapSelection { get; set; } = false;
    public IReadOnlyList<MapData> MapData => data;
    [SerializeField] private MapData[] data = null;
    [SerializeField] private MapGenerator generator = null;

    private MapData lastMapData;

    public void Generate(MapData mapData)
    {
        lastMapData = mapData;
        Map = generator.Generate(mapData);
    }

    public void Regenerate()
    {
        Map = generator.Generate(lastMapData);
    }

    public void Clear()
    {
        Map = null;
    }
}
