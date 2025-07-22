using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterGeneratorStep : ScriptableObject
{
    public abstract void Apply(ref Encounter encounter, ref HashSet<Vector2Int> validPositions);
}
