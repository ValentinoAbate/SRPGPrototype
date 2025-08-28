using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Units Weighted (Distance from Pos)")]
public class EncounterStepPlaceUnitsWeightedDistanceFromPos : EncounterStepPlaceUnitsWeighted
{
    [SerializeField] private Vector2Int targetPosition;
    [SerializeField] private bool useColumnDist;
    [SerializeField] private bool useRowDist;
    [SerializeField] private float weightExponent = 2;

    protected override void PlaceUnits(int numUnits, WeightedSet<Unit> units, Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        PlaceUnitsWeighted(numUnits, units, PositionWeight, encounter, ref validPositions);
    }

    private float PositionWeight(Vector2Int pos, Encounter encounter)
    {
        float score = 0;
        if (useColumnDist)
        {
            score += (encounter.dimensions.x - 1) - System.Math.Abs(pos.x - targetPosition.x);
        }
        if (useRowDist)
        {
            score += (encounter.dimensions.y - 1) - System.Math.Abs(pos.y - targetPosition.y);
        }
        return Mathf.Pow(score, weightExponent);
    }
}
