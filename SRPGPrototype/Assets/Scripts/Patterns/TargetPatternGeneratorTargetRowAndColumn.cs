using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorTargetRowAndColumn : TargetPatternGenerator
{
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        int i = 1;
        while (true)
        {
            bool done = true;
            var right = targetPos + (Vector2Int.right * i);
            if (grid.IsLegal(right))
            {
                done = false;
                yield return right;
            }
            var left = targetPos + (Vector2Int.left * i);
            if (grid.IsLegal(left))
            {
                done = false;
                yield return left;
            }
            var up = targetPos + (Vector2Int.up * i);
            if (grid.IsLegal(up))
            {
                done = false;
                yield return up;
            }
            var down = targetPos + (Vector2Int.down * i);
            if (grid.IsLegal(down))
            {
                done = false;
                yield return down;
            }
            if (done)
            {
                break;
            }
            ++i;
        }
    }
}
