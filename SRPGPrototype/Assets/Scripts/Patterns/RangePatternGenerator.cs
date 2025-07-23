using System.Collections.Generic;
using UnityEngine;

public abstract class RangePatternGenerator : MonoBehaviour
{
    public abstract IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user);

    public virtual IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        return Generate(grid, targetPos, user);
    }

    public abstract int MaxDistance(BattleGrid grid);
}
