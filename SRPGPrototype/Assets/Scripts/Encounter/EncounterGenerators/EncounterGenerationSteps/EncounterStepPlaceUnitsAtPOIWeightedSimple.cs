using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Units at POI")]
public class EncounterStepPlaceUnitsAtPOIWeightedSimple : EncounterStepPlaceUnitsWeighted
{
    [SerializeField] private Metadata.PointsOfInterest pointsOfInterest;
    [SerializeField] private BooleanOperator booleanOperator;
    protected override void PlaceUnits(int numUnits, WeightedSet<Unit> units, Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        var positions = new List<Vector2Int>();
        foreach(var kvp in metadata.pointsOfInterest)
        {
            if (encounter.HasUnitAt(kvp.Key))
                continue;
            if(booleanOperator.Evaluate(kvp.Value, pointsOfInterest))
            {
                positions.Add(kvp.Key);
            }
        }
        for(int i = 0; i < numUnits; ++i)
        {
            if (positions.Count <= 0)
                return;
            var pos = RandomU.instance.Choice(positions, out int selectedIndex);
            positions.RemoveAt(selectedIndex);
            encounter.AddUnit(RandomU.instance.Choice(units), pos);
        }
    }
}
