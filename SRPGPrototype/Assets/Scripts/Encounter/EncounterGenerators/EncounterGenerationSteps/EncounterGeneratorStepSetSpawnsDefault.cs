using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Set Spawn Points (Default)")]
public class EncounterGeneratorStepSetSpawnsDefault : EncounterGeneratorStep
{
    [SerializeField] private int num;
    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        ChooseSpawnPositionsDefault(num, encounter, ref validPositions);
    }
}
