using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierTargetPattern : ModifierAction
{
    public override bool AppliesTo(SubAction sub)
    {
        return base.AppliesTo(sub) && AppliesToPattern(sub.TargetType, sub.TargetPatternGenerator);
    }

    public abstract IEnumerable<Vector2Int> Modify(TargetPattern t, BattleGrid grid, Unit user, Vector2Int targetPos, RangePattern range);

    protected abstract bool AppliesToPattern(TargetPattern.Type t, TargetPatternGenerator generator);

    protected static IEnumerable<Vector2Int> UniqueWithSelector(IEnumerable<Vector2Int> positions, System.Func<Vector2Int, Vector2Int> selector, System.Predicate<Vector2Int> pred = null)
    {
        var visited = new HashSet<Vector2Int>();
        foreach (var pos in positions)
        {
            if (pred != null && !pred(pos))
            {
                visited.Add(pos);
                yield return pos;
                continue;
            }
            if (!visited.Contains(pos))
            {
                visited.Add(pos);
                yield return pos;
            }
            var modPos = selector(pos);
            if (!visited.Contains(modPos))
            {
                visited.Add(modPos);
                yield return modPos;
            }
        }
    }

}
