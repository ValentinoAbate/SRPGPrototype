using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierTargetPatternMirror : ModifierTargetPattern
{
    public override IEnumerable<Vector2Int> Modify(TargetPattern t, BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        var visited = new HashSet<Vector2Int>();
        foreach (var pos in t.Target(grid, user, targetPos))
        {
            if (!visited.Contains(pos))
            {
                visited.Add(pos);
                yield return pos;
            }
            var modPos = user.Pos - (pos - user.Pos);
            if (!visited.Contains(modPos))
            {
                visited.Add(modPos);
                yield return modPos;
            }
        }
    }

    protected override bool AppliesToPattern(TargetPattern.Type t)
    {
        return t != TargetPattern.Type.Self;
    }
}
