using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Units Random")]
public class EncounterStepPlaceUnitsRandom : EncounterGeneratorStep
{
    [SerializeField] private List<Unit> units = new List<Unit>();

    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        foreach(var unit in units)
        {
            if (validPositions.Count <= 0)
                return;
            AddUnit(unit, RandomU.instance.Choice(validPositions), ref encounter, ref validPositions);
        }
    }
}
