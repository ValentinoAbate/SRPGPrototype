using RandomUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterSetGenerator", menuName = "Encounter Generation/Set Generator (Weighted)")]
public class EncounterSetGeneratorRandomWeighted : EncounterSetGenerator
{
    [SerializeField] private List<EncounterGenerator> generators = new List<EncounterGenerator>();
    [SerializeField] private List<float> generatorWeights = new List<float>();

    protected WeightedSet<EncounterGenerator> generatorSet;

    protected override void Initialize()
    {
        base.Initialize();
        generatorSet = WeightedSetUtils.GetSetWeightsOptional(generators, generatorWeights);
    }

    protected override IReadOnlyList<Encounter> GenerateEncountersInternal(string mapSymbol, int encounterNumber, Metadata metadata)
    {
        int numEncounters = Mathf.Clamp(RandomU.instance.Choice(numEncountersSet), minEncounters, maxEncounters);
        var output = new List<Encounter>(numEncounters);
        for (int i = 0; i < numEncounters; i++)
        {
            output.Add(RandomU.instance.Choice(generatorSet).Generate(mapSymbol, encounterNumber, metadata));
        }
        return output;
    }
}
