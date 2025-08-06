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
            var directions = new List<Vector2Int>(3);
            directions.Add(Vector2Int.one);
            if(startingPos.x != 0 && startingPos.x != encounter.dimensions.x - 1)
            {
                directions.Add(Vector2Int.up);
            }
            if(startingPos.y != 0 && startingPos.y != encounter.dimensions.y - 1)
            {
                directions.Add(Vector2Int.right);
            }
            var direction = RandomU.instance.Choice(directions);
            int wallsLeft = wallLength - 1;
            for(int j = 1; j < wallLength; ++j)
            {
                var targetPos = startingPos + (direction * j);
                if (validPositions.Contains(targetPos))
                {
                    AddUnit(wallUnit, targetPos, ref encounter, ref validPositions);
                    wallsLeft--;
                }
                targetPos = startingPos - (direction * j);
                if (wallsLeft <= 0)
                    break;
                if (validPositions.Contains(targetPos))
                {
                    AddUnit(wallUnit, targetPos, ref encounter, ref validPositions);
                    wallsLeft--;
                }
                if (wallsLeft <= 0)
                    break;
            }
        }
    }
}
