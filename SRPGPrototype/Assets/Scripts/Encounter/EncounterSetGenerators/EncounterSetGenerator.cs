using RandomUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterSetGenerator : ScriptableObject
{
    protected const int minEncounters = 1;
    protected const int maxEncounters = 8;

    private static readonly IReadOnlyCollection<KeyValuePair<int, float>> defaultNumEncounterSet = new WeightedSet<int>(
    new int[]
    {
            2,3,4,5,6
    },
    new float[]
    {
            10,20,10,5,1
    });

    [SerializeField] private List<int> numEncounterOptions = new List<int>();
    [SerializeField] private List<float> numEncounterWeights = new List<float>();
    [SerializeField] private NPCSet npcSet;

    [NonSerialized] protected IReadOnlyCollection<KeyValuePair<int, float>> numEncountersSet;
    [NonSerialized] private bool initialized = false;

    protected virtual void Initialize()
    {
        if (numEncounterOptions.Count > 0)
        {
            numEncountersSet = WeightedSetUtils.GetSetWeightsOptional(numEncounterOptions, numEncounterWeights);
        }
        else
        {
            numEncountersSet = defaultNumEncounterSet;
        }
    }

    public IReadOnlyList<Encounter> GenerateEncounters(string mapSymbol, int encounterNumber)
    {
        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
        return GenerateEncountersInternal(mapSymbol, encounterNumber, GenerateMetadata());
    }

    protected abstract IReadOnlyList<Encounter> GenerateEncountersInternal(string mapSymbol, int encounterNumber, Metadata metadata);

    protected Metadata GenerateMetadata()
    {
        var metadata = new Metadata();
        if(npcSet != null)
        {
            npcSet.GetNPCs(ref metadata.primaryNPCs, ref metadata.secondaryNPCs);
        }
        return metadata;
    }



    public class Metadata
    {
        public WeightedSet<Unit> primaryNPCs = null;
        public WeightedSet<Unit> secondaryNPCs = null;
    }
}
