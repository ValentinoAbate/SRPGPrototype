using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Special/Place Pawns")]
public class EncounterStepPlacePawns : EncounterGeneratorStep
{
    [SerializeField] private List<int> numUnits = new List<int>();
    [SerializeField] private List<float> numUnitsWeights = new List<float>();
    [SerializeField] private Unit pawnPrefab;

    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        int numUnitsChoice = RandomU.instance.ChoiceWeightsOptional(numUnits, numUnitsWeights);
        var columns = new WeightedSet<int>(Enumerable.Range(0, encounter.dimensions.x));
        if (columns.Count <= 0)
            return;
        var rows = new WeightedSet<int>(encounter.dimensions.y);
        rows.Add(1, 2);
        for (int i = 2; i < encounter.dimensions.y; i++)
        {
            rows.Add(i, Mathf.Pow(encounter.dimensions.y - i, 2));
        }
        if (rows.Count <= 0)
            return;
        for (int i = 0; i < numUnitsChoice; i++)
        {
            if (validPositions.Count <= 0)
                return;
            int column = RandomU.instance.Choice(columns);
            int row = RandomU.instance.Choice(rows);
            var pos = new Vector2Int(column, row);
            // TODO: better method of ensuring pos is valid
            if (!validPositions.Contains(pos))
                continue;
            AddUnit(pawnPrefab, pos, ref encounter, ref validPositions);
            rows.Multiply(row, 0.33f);
            columns.Multiply(column, 0.15f);
        }
    }
}
