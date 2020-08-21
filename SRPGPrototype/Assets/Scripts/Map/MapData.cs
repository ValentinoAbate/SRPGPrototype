using RandomUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map Generation Data")]
public class MapData : ScriptableObject
{
    public const int defaultDepth = 10;
    public int Depth => depth;
    [SerializeField] private int depth = defaultDepth;
    [SerializeField] private List<Entry> encounterData = new List<Entry>(defaultDepth);
    [SerializeField] private List<EncounterData> testData = new List<EncounterData>(defaultDepth);

    public WeightedSet<EncounterData> GetEncounterData(int depth)
    {
        if (depth >= Depth)
            return null;
        var entry = encounterData[depth];
        return new WeightedSet<EncounterData>(entry.data, entry.weights);
        //return new WeightedSet<EncounterData>(testData, 1);
    }
    [Serializable]
    public class Entry
    {
        public List<EncounterData> data = new List<EncounterData>();
        public List<float> weights = new List<float>();
    }
}
