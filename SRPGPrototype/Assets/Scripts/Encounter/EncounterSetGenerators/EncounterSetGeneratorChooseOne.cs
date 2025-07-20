using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSetGeneratorChooseOne : EncounterSetGeneratorRandomWeighted
{
    protected override IReadOnlyList<Encounter> GenerateEncountersInternal(string mapSymbol, int encounterNumber)
    {
        int numEncounters = Mathf.Clamp(RandomU.instance.Choice(numEncountersSet), minEncounters, maxEncounters);
        var encounterGenerator = RandomU.instance.Choice(generatorSet);
        var output = new List<Encounter>(numEncounters);
        for (int i = 0; i < numEncounters; i++)
        {
            output.Add(encounterGenerator.Generate(mapSymbol, encounterNumber));
        }
        return output;
    }
}
