using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Units Weighted (Dist from Unit with Tags)")]
public class EncounterStepPlaceUnitsWeightedDistanceFromUnitWithTags : EncounterStepPlaceUnitsWeighted
{
    [SerializeField] private Unit.Tags tags;
    [SerializeField] private BooleanOperator tagComparison = BooleanOperator.AND; 
    [SerializeField] private float weightExponent = 2;

    protected override void PlaceUnits(int numUnits, WeightedSet<Unit> units, Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        PlaceUnitsWeighted(numUnits, units, PositionWeight, encounter, ref validPositions);
    }

    private float PositionWeight(Vector2Int pos, Encounter encounter)
    {
        float score = 0;
        bool bossUnitFound = false;
        foreach(var unit in encounter.Units)
        {
            if(tagComparison.Evaluate(unit.unit.UnitTags, tags))
            {
                score += (encounter.dimensions.x - 1) + (encounter.dimensions.y - 1) - pos.GridDistance(unit.pos);
                bossUnitFound = true;
            }
        }
        if (!bossUnitFound)
            return 1;
        return Mathf.Pow(score, weightExponent);
    }
}
