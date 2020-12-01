﻿using System.Collections.Generic;
using UnityEngine;

public abstract class RangePatternGenerator : MonoBehaviour
{
    public abstract IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos);

    public abstract int MaxDistance(BattleGrid grid);
}
