using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetPatternGenerator : MonoBehaviour
{
    public abstract List<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos);
}
