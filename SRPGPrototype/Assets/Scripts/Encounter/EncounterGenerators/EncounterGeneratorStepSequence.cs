using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter Generation/Encounter Gen (Step Sequence)")]
public class EncounterGeneratorStepSequence : EncounterGenerator
{
    [SerializeField] public float targetDifficulty;
    [SerializeField] private List<EncounterGeneratorStep> steps;
    [Header("Seed Properties")]
    [SerializeField] private Encounter seed;
    public override Encounter Generate(string mapSymbol, int encounterNumber, EncounterSetGenerator.Metadata metadata)
    {
        var stepMetadata = new EncounterGeneratorStep.Metadata()
        {
            targetDifficulty = targetDifficulty,
            primaryNPCs = metadata.primaryNPCs,
            secondaryNPCs = metadata.secondaryNPCs,
        };
        InitializeEncounter(mapSymbol, encounterNumber, out var positions, out var encounter);
        ApplySeed(seed, ref encounter, ref positions);
        foreach(var step in steps)
        {
            step.Apply(stepMetadata, ref encounter, ref positions);
        }
        return encounter;
    }
}
