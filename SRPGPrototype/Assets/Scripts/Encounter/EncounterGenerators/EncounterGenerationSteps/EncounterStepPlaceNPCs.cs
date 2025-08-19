using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place NPCs")]
public class EncounterStepPlaceNPCs : EncounterGeneratorStep
{
    [SerializeField] private List<int> numNPCs = new List<int>();
    [SerializeField] private List<float> numNPCsWeights = new List<float>();
    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        if (validPositions.Count <= 0)
            return;
        bool primaryAvailable = metadata.primaryNPCs != null && metadata.primaryNPCs.Count > 0;
        bool secondaryAvailable = metadata.secondaryNPCs != null && metadata.secondaryNPCs.Count > 0;
        if (!primaryAvailable && !secondaryAvailable)
            return;
        int num = RandomU.instance.ChoiceWeightsOptional(numNPCs, numNPCsWeights);
        if (num > 1 && (!primaryAvailable || !secondaryAvailable))
            --num;
        for (int i = 0; i < num; i++)
        {
            if (!secondaryAvailable || RandomU.instance.RollSuccess(PrimaryChance(i)))
            {
                // If secondary is not available, primary must be available
                Choose(metadata.primaryNPCs, ref encounter, ref validPositions);
            }
            else
            {
                // Secondary must be available to hit this branch
                Choose(metadata.secondaryNPCs, ref encounter, ref validPositions);
            }
            if (validPositions.Count <= 0)
                return;
            primaryAvailable = primaryAvailable && metadata.primaryNPCs.Count > 0;
            secondaryAvailable = secondaryAvailable && metadata.secondaryNPCs.Count > 0;
            if (!primaryAvailable && !secondaryAvailable)
                return;
        }
    }

    private void Choose(WeightedSet<Unit> options, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        var unit = RandomU.instance.Choice(options);
        options.Remove(unit);
        var pos = RandomU.instance.Choice(validPositions);
        validPositions.Remove(pos);
        encounter.AddUnit(unit, pos);
    }

    private static double PrimaryChance(int index) => index <= 1 ? 0.95 : 0.05;
}
