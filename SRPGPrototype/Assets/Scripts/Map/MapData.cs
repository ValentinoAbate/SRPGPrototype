using RandomUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map Generation Data")]
public class MapData : ScriptableObject, IHasKey
{
    public string Key => key;
    [SerializeField] private string key;
    public string MapName => mapName;
    [SerializeField] private string mapName;
    [SerializeField] private string mapSymbol;
    public MapData NextMap => nextMap;
    [SerializeField] private MapData nextMap;
    [SerializeField] private List<EncounterSetGenerator> encounterSetGenerators;
    [Header("Legacy")]
    [SerializeField] private List<Entry> encounterData = new List<Entry>(10);

    public bool TryGetEncounterSet(int depth, out IReadOnlyList<Encounter> encounterSet)
    {
        if (depth < 0 || depth >= encounterSetGenerators.Count)
        {
            encounterSet = Array.Empty<Encounter>();
            return false;
        }
        encounterSet = encounterSetGenerators[depth].GenerateEncounters(mapSymbol, depth + 1);
        return true;
    }

    // legacy data for reference
    [Serializable]
    public class Entry
    {
        public List<EncounterGeneratorBasic> data = new List<EncounterGeneratorBasic>();
        public List<float> weights = new List<float>();
    }

#if UNITY_EDITOR

    public bool GenerateKey()
    {
        return false; 
    }
#endif
}
