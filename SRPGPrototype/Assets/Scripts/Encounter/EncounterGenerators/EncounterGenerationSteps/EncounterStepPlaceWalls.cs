using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Walls")]
public class EncounterStepPlaceWalls : EncounterGeneratorStep
{
    [SerializeField] private List<int> numWalls = new List<int>();
    [SerializeField] private List<float> numWallsWeights = new List<float>();
    [SerializeField] private List<Unit> units = new List<Unit>();
    [SerializeField] private List<float> unitChoiceWeights = new List<float>();
    [SerializeField] private double wallLengthAvg;
    [SerializeField] private double wallLengthStdDeviation;

    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        int num = RandomU.instance.ChoiceWeightsOptional(numWalls, numWallsWeights);
        for (int i = 0; i < num; i++)
        {
            if (validPositions.Count <= 0)
                return;
            int wallLength = RandomU.instance.RandomGaussianInt(wallLengthAvg, wallLengthStdDeviation);
            if (wallLength <= 1)
                continue;
            var wallUnit = RandomU.instance.ChoiceWeightsOptional(units, unitChoiceWeights);
            var startingPos = RandomU.instance.Choice(validPositions);
            AddUnit(wallUnit, startingPos, ref encounter, ref validPositions);
            var direction = RandomU.instance.RandomBool() ? Vector2Int.up : Vector2Int.left;
            for(int j = 1; j < wallLength; ++j)
            {
                var targetPos = startingPos + (direction * j);
                if (!validPositions.Contains(targetPos)) // could create long walls (valid positions might be filled with other units)
                {
                    targetPos = startingPos - (direction * j);
                }
                if (!validPositions.Contains(targetPos))
                {
                    continue;
                }
                direction *= -1;
                AddUnit(wallUnit, targetPos, ref encounter, ref validPositions);
            }
        }
    }
}
