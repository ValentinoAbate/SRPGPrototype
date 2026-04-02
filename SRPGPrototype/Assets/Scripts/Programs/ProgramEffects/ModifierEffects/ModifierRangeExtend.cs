using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierRangeExtend : ModifierRange
{
    public override IEnumerable<Vector2Int> Modify(RangePattern r, BattleGrid grid, Unit user, Vector2Int origin)
    {
        Vector2Int Selector(Vector2Int pos) => pos + origin.DirectionTo(pos);
        bool Predicate(Vector2Int pos) => pos != user.Pos;
        foreach (var pos in Vector2IntUtils.UniqueWithSelector(r.GetPositions(grid, origin, user), Selector, Predicate))
        {
            yield return pos;
        }
    }

    protected override bool AppliesToPattern(RangePattern.Type t, RangePatternGenerator generator)
    {
        return t != RangePattern.Type.Self && (t != RangePattern.Type.Generated || IsSupportedGenerator(generator));
    }

    private bool IsSupportedGenerator(RangePatternGenerator generator)
    {
        return generator is RangePatternGeneratorDirectionalRaycastOver
            || generator is RangePatternGeneratorTunnel
            || generator is RangePatternGeneratorClockwise;
    }
}
