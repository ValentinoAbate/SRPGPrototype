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
    [SerializeField] private List<List<EncounterData>> encounterData = new List<List<EncounterData>>(defaultDepth);
    [SerializeField] private List<EncounterData> testData = new List<EncounterData>(defaultDepth);

    public IEnumerable<EncounterData> GetEncounterData(int depth)
    {
        if (depth >= this.depth)
            return null;
        return testData;
        return encounterData[depth];
    }
}
