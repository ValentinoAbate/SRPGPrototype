using Extensions.VectorIntDimensionUtils;
using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Rooms")]
public class EncounterStepPlaceRooms : EncounterGeneratorStep
{
    [SerializeField] private List<int> numRooms = new List<int>();
    [SerializeField] private List<float> numRoomsWeights = new List<float>();
    [SerializeField] private List<int> roomSizes = new List<int>();
    [SerializeField] private List<float> roomSizesWeights = new List<float>();
    [SerializeField] private Unit wallUnit;
    [SerializeField] private Unit cornerUnit;

    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        int num = RandomU.instance.ChoiceWeightsOptional(numRooms, numRoomsWeights);
        if (num <= 0)
            return;
        var adjacentTiles = new WeightedSet<Vector2Int>();
        var roomInterior = new List<Vector2Int>();
        var dimensions = encounter.dimensions;
        bool IsOnEdge(Vector2Int vec) => vec.IsOnEdge(dimensions);
        bool CantBeDoor(Vector2Int vec) => IsOnEdge(vec) || adjacentTiles[vec] > 1;
        for (int i = 0; i < num; i++)
        {
            if (validPositions.Count <= 0)
                return;
            var startPos = RandomU.instance.Choice(validPositions);
            roomInterior.Clear();
            roomInterior.Add(startPos);
            validPositions.Remove(startPos);
            metadata.SetPointOfInterest(startPos, Metadata.PointsOfInterest.RoomInterior);
            int roomSize = RandomU.instance.ChoiceWeightsOptional(roomSizes, roomSizesWeights);
            // Generate room interior
            adjacentTiles.Clear();
            foreach (var adj in startPos.Adjacent())
            {
                if (validPositions.Contains(adj))
                    adjacentTiles.Add(adj);
            }
            if (roomSize > 1)
            {
                for (int j = 1; j < roomSize; j++)
                {
                    if (adjacentTiles.Count <= 0)
                        break;
                    var pos = RandomU.instance.Choice(adjacentTiles);
                    adjacentTiles.Remove(pos);
                    roomInterior.Add(pos);
                    validPositions.Remove(pos);
                    metadata.SetPointOfInterest(pos, Metadata.PointsOfInterest.RoomInterior);
                    foreach (var adj in pos.Adjacent())
                    {
                        if (validPositions.Contains(adj))
                            adjacentTiles.Add(adj);
                    }
                }
            }
            adjacentTiles.RemoveWhere(CantBeDoor);
            Vector2Int doorPos = BattleGrid.OutOfBounds;
            if(adjacentTiles.Count > 0)
            {
                doorPos = RandomU.instance.Choice(adjacentTiles);
                validPositions.Remove(doorPos);
            }
            // Generate walls
            foreach (var pos in roomInterior)
            {
                foreach(var adj in pos.Adjacent())
                {
                    if(validPositions.Contains(adj))
                    {
                        encounter.AddUnit(wallUnit, adj);
                        validPositions.Remove(adj);
                    }
                }
            }
            // Generate corners
            foreach (var pos in roomInterior)
            {
                foreach (var adj in pos.AdjacentDiagonal())
                {
                    if (validPositions.Contains(adj))
                    {
                        encounter.AddUnit(cornerUnit, adj);
                        validPositions.Remove(adj);
                    }
                }
            }
            if(doorPos != BattleGrid.OutOfBounds)
            {
                foreach (var adj in doorPos.Adjacent())
                {
                    if (validPositions.Contains(adj))
                    {
                        validPositions.Remove(adj);
                    }
                }
            }
        }
    }
}
