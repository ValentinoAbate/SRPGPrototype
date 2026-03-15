using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierTargetPatternExtend : ModifierTargetPattern
{
    public override IEnumerable<Vector2Int> Modify(TargetPattern t, BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        if(t.patternType == TargetPattern.Type.Simple)
        {
            yield return targetPos;
            yield return targetPos + user.Pos.DirectionTo(targetPos);
        }
        if(t.patternType == TargetPattern.Type.Pattern)
        {
            var visited = new HashSet<Vector2Int>();
            foreach (var pos in t.Target(grid, user, targetPos))
            {
                if (pos == targetPos)
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
                var modPos = pos + targetPos.DirectionTo(pos);
                if (!visited.Contains(modPos))
                {
                    visited.Add(modPos);
                    yield return modPos;
                }
            }
        }
        else if(t.patternType == TargetPattern.Type.DirectionalPattern || t.patternType == TargetPattern.Type.DirectionalPatternShift1 || t.patternType == TargetPattern.Type.DirectionalPatternAndSelf)
        {
            var direction = user.Pos.DirectionTo(targetPos);
            var visited = new HashSet<Vector2Int>();
            var positions = new List<Vector2Int>(t.Target(grid, user, targetPos));
            int Comparer(Vector2Int v1, Vector2Int v2)
            {
                return user.Pos.GridDistance(v1).CompareTo(user.Pos.GridDistance(v2));
            }
            positions.Sort(Comparer);
            var lookup = new HashSet<Vector2Int>();
            foreach (var pos in positions)
            {
                lookup.Add(pos);
            }
            foreach (var pos in positions)
            {
                yield return pos;
                if (visited.Contains(pos))
                    continue;
                visited.Add(pos);
                if(pos == user.Pos)
                {
                    continue;
                }
                foreach(var exPos in ExtendPoint(pos, pos - direction, direction, lookup, visited))
                {
                    yield return exPos;
                }
            }       
        }
    }

    private IEnumerable<Vector2Int> ExtendPoint(Vector2Int pos, Vector2Int source, Vector2Int direction, HashSet<Vector2Int> lookup, HashSet<Vector2Int> visited)
    {
        var queue = new Queue<Vector2Int>();
        foreach (var pAdj in SearchOrder(pos, direction))
        {
            if (visited.Contains(pAdj) || !lookup.Contains(pAdj))
                continue;
            queue.Enqueue(pAdj);
            visited.Add(pAdj);
        }
        if(queue.Count <= 0)
        {
            yield return pos + source.DirectionTo(pos);
            yield break;
        }
        while (queue.Count > 0)
        {
            var pAdj = queue.Dequeue();
            foreach (var exPos in ExtendPoint(pAdj, pos, direction, lookup, visited))
            {
                yield return exPos;
            }
        }
    }

    private IEnumerable<Vector2Int> SearchOrder(Vector2Int pos, Vector2Int targetDirection)
    {
        yield return pos + targetDirection;
        yield return pos + new Vector2Int(targetDirection.y, -targetDirection.x);
        yield return pos + new Vector2Int(-targetDirection.y, targetDirection.x);
        yield return pos + new Vector2Int(-targetDirection.x, -targetDirection.y);

        // Cardinal
        int abs = Mathf.Abs(targetDirection.x + targetDirection.y);
        if (abs == 1)
        {
            yield return pos + targetDirection + new Vector2Int(targetDirection.y, -targetDirection.x);
            yield return pos + targetDirection - new Vector2Int(targetDirection.y, -targetDirection.x);
            yield return pos - targetDirection + new Vector2Int(-targetDirection.y, targetDirection.x);
            yield return pos - targetDirection - new Vector2Int(targetDirection.y, -targetDirection.x);
        }
        else
        {
            var rot = abs == 2 ? new Vector2Int(targetDirection.x, 0) : new Vector2Int(0, targetDirection.y);
            var rot2 = abs == 2 ? new Vector2Int(0, targetDirection.y) : new Vector2Int(targetDirection.x, 0);
            yield return pos + (targetDirection * rot);
            yield return pos + (targetDirection * rot2);
            yield return pos - (targetDirection * rot2);
            yield return pos - (targetDirection * rot);
        }
    }

    protected override bool AppliesToPattern(TargetPattern.Type t)
    {
        return t == TargetPattern.Type.DirectionalPattern
            || t == TargetPattern.Type.DirectionalPatternShift1
            || t == TargetPattern.Type.DirectionalPatternAndSelf
            || t == TargetPattern.Type.Simple
            || t == TargetPattern.Type.Pattern;
    }
}
