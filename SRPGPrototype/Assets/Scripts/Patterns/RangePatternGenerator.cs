using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangePatternGenerator : MonoBehaviour
{
    public abstract List<Vector2Int> Generate(BattleGrid grid, Unit user);
}
