﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the data from the maps, including the current encounter
/// </summary>
public class MapManager : MonoBehaviour
{
    public Encounter CurrentEncounter { get; set; }
    public IReadOnlyList<Encounter> NextEncounters { get; private set; }
    public int BaseMapDepth { get; private set; }
    public bool SkipMapSelection { get; set; } = false;
    public IReadOnlyList<MapData> StartingMaps => startingMaps;
    [SerializeField] private MapData[] startingMaps = null;

    private MapData lastStartingMapData;
    private readonly Stack<MapDataEntry> mapStack = new Stack<MapDataEntry>();

    public void Setup(MapData startingMap)
    {
        lastStartingMapData = startingMap;
        mapStack.Clear();
        BaseMapDepth = 0;
        mapStack.Push(new MapDataEntry(startingMap, true));
        GenerateNextEncounters();
    }

    public void Restart()
    {
        Setup(lastStartingMapData);
    }

    public void GenerateNextEncounters()
    {
        while(mapStack.Count > 0)
        {
            var mapEntry = mapStack.Peek();
            var map = mapEntry.data;
            if (map.TryGetEncounterSet(mapEntry.isBaseMap ? BaseMapDepth : mapEntry.progress, out var encounters))
            {
                ++BaseMapDepth;
                ++mapEntry.progress;
                NextEncounters = encounters;
                return;
            }
            mapStack.Pop();
            if(map.NextMap != null)
            {
                if (mapEntry.isBaseMap)
                {
                    BaseMapDepth = 0;
                }
                mapStack.Push(new MapDataEntry(map.NextMap, mapEntry.isBaseMap));
            }
        }
        NextEncounters = System.Array.Empty<Encounter>();
        return;
    }

    private class MapDataEntry
    {
        public MapData data;
        public int progress = 0;
        public bool isBaseMap;

        public MapDataEntry(MapData data, bool isBaseMap)
        {
            this.data = data;
            this.isBaseMap = isBaseMap;
        }
    }
}
