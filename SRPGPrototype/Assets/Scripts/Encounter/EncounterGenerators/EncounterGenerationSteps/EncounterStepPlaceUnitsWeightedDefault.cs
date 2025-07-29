using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Units Weighted (Default)")]
public class EncounterStepPlaceUnitsWeightedDefault : EncounterStepPlaceUnitsWeighted
{
    protected override void PlaceUnits(int numUnits, WeightedSet<Unit> units, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        // Place half rounded up totally randomly
        PlaceUnitsRandom(numUnits / 2 + numUnits % 2, units, ref encounter, ref validPositions);
        // Place half rounded down using clump weight
        PlaceUnitsWeighted(numUnits / 2, units, ClumpWeight, encounter, ref validPositions);
    }
}
