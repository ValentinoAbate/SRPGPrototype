using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class AIComponent<T> : MonoBehaviour where T : Unit
{
    // amount of time to pause for each square moved
    public const float moveDelay = 0.25f;
    public const float attackDelay = 0.5f;

    public abstract List<Action> Actions { get; }

    public abstract IEnumerator DoTurn(BattleGrid grid, T self);

    public int CompareTargetPriority(Unit obj1, int pathDist1, Unit obj2, int pathDist2)
    {
        // First compare grid distance
        int distCmp = pathDist1.CompareTo(pathDist2);
        if (distCmp != 0)
            return distCmp;
        // If grid distance is the same, compare hp
        if (obj1.HP != obj2.HP)
            return obj1.HP.CompareTo(obj2.HP);
        // Later will compare based on explicit sorting order (names)
        return 0;
    }

    protected int GetPathDist(BattleGrid grid, Vector2Int start, Vector2Int goal)
    {
        var path = grid.Path(start, goal, (u) => u == null || u.Pos == start || u.Pos == goal);
        if (path == null)
            return int.MaxValue;
        return path.Count;
    }

    protected int GetPathDist(BattleGrid grid, Vector2Int start, Vector2Int goal, Unit.Team canMoveThrough)
    {
        var path = grid.Path(start, goal, (u) => u == null || u.UnitTeam == canMoveThrough || u.Pos == start || u.Pos == goal);
        if (path == null)
            return int.MaxValue;
        return path.Count;
    }

    protected IEnumerator MoveAlongPath(BattleGrid grid, T self, List<Vector2Int> path, int maxDistance = int.MaxValue)
    {
        // Move along the path until within range
        for (int i = 0; i < maxDistance && i < path.Count; ++i)
        {
            //yield return new WaitWhile(() => self.PauseHandle.Paused);
            grid.MoveAndSetWorldPos(self, path[i]);
            yield return new WaitForSeconds(moveDelay);
        }
    }

    protected IEnumerator MoveToTargetRange<Target>(BattleGrid grid, T self, Action moveAction, Action standardAction, List<Target> targets) where Target : Unit
    {
        var sub = moveAction.subActions[0];
        var movePositions = sub.Range.GetPositions(grid, self)
            .Where((p) => grid.IsLegal(p) && grid.IsEmpty(p)).ToList();
        movePositions.Add(self.Pos);
        // Move into a viable attack position if possible
        foreach (var pos in movePositions)
        {
            // If our standard action could hit a target form the potential location
            if (CheckforTargets(grid, self, standardAction, targets) != BattleGrid.OutOfBounds)
            {
                moveAction.StartAction(self);
                sub.Use(grid, moveAction, self, pos);
                moveAction.FinishAction(self);
                yield return new WaitForSeconds(moveDelay);
                yield break;
            }
        }
        var posByPathDist = movePositions.Select((p) => new PosValuePair(p, targets.Min((unit) => GetPathDist(grid, p, unit.Pos))));
        // Else find the position that can be moved to that brings us closest to a player unit
        if (posByPathDist.Count() <= 0)
            yield break;
        var closest = posByPathDist.OrderBy((t) => t.value).First();
        if (closest.value == int.MaxValue)
        {
            posByPathDist = movePositions.Select((p) => new PosValuePair(p, targets.Min((unit) => GetPathDist(grid, p, unit.Pos, unit.UnitTeam))));
            // Else find the position that can be moved to that brings us closest to a player unit
            if (posByPathDist.Count() <= 0)
                yield break;
            closest = posByPathDist.OrderBy((t) => t.value).First();
        }
        if (closest.value == int.MaxValue)
        {
            var posByDist = movePositions.Select((p) => new PosValuePair(p, targets.Min((unit) => (int)Vector2Int.Distance(unit.Pos, p))));
            if (posByDist.Count() <= 0)
                yield break;
            closest = posByDist.OrderBy((t) => t.value).First();
        }

        moveAction.StartAction(self);
        sub.Use(grid, moveAction, self, closest.pos);
        moveAction.FinishAction(self);
        yield return new WaitForSeconds(moveDelay);
    }

    protected Vector2Int CheckforTargets<Target>(BattleGrid grid, Unit self, Action standardAction, List<Target> targetUnits) where Target : Unit
    {
        var subAction = standardAction.subActions[0];
        foreach (var pos in subAction.Range.GetPositions(grid, self))
        {
            var targetPositions = subAction.targetPattern.Target(grid, self, pos);
            foreach (var tPos in targetPositions)
            {
                if (targetUnits.Any((u) => u.Pos == tPos))
                {
                    return pos;
                }
            }
        }
        return BattleGrid.OutOfBounds;
    }

    protected List<Unit> FindTargetsOnTeams(BattleGrid grid, List<Unit.Team> teams)
    {
        return grid.FindAll<Unit>((c) => teams.Any((team) => c.UnitTeam == team));
    }

    private struct PosValuePair
    {
        public Vector2Int pos;
        public int value;
        public PosValuePair(Vector2Int pos, int value)
        {
            this.pos = pos;
            this.value = value;
        }
    }
}
