using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterStepPlaceUnitsWeighted : EncounterGeneratorStep
{
    [SerializeField] private List<int> numUnits = new List<int>();
    [SerializeField] private List<float> numUnitsWeights = new List<float>();
    [SerializeField] private List<Unit> units = new List<Unit>();
    [SerializeField] private List<float> unitChoiceWeights = new List<float>();

    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        int numUnitsChoice = RandomU.instance.ChoiceWeightsOptional(numUnits, numUnitsWeights);
        var unitSet = WeightedSetUtils.GetSetWeightsOptional(units, unitChoiceWeights);
        PlaceUnits(numUnitsChoice, unitSet, ref encounter, ref validPositions);
    }

    protected abstract void PlaceUnits(int numUnits, WeightedSet<Unit> units, ref Encounter encounter, ref HashSet<Vector2Int> validPositions);
}
