using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AIComponent<T> : MonoBehaviour where T : AIUnit
{
    // amount of time to pause for each square moved
    public const float moveDelay = 0.25f;
    public const float attackDelay = 0.5f;

    public abstract List<Action> Actions { get; }

    public abstract void Initialize(T self);

    public abstract IEnumerator DoTurn(BattleGrid grid, T self);

    protected IEnumerator MoveAlongPath(BattleGrid grid, T self, Action moveAction, List<Vector2Int> path)
    {
        // Move along the path
        foreach (var pos in path)
        {
            // If we can't move any further, end the turn
            if (!self.CanUseAction(moveAction))
                yield break;
            moveAction.UseAll(grid, self, pos);
            yield return new WaitForSeconds(moveDelay);
        }
    }

    protected IEnumerator AttackUntilExhausted(BattleGrid grid, AIUnit self, Action standardAction, Vector2Int tPos)
    {
        while (self.CanUseAction(standardAction))
        {
            standardAction.UseAll(grid, self, tPos);
            yield return new WaitForSeconds(attackDelay);
            Debug.Log(self.DisplayName + " is targeting tile: " + tPos.ToString() + " for an attack!");
        }
    }

    protected IEnumerator PathThenAttackIfAble(BattleGrid grid, T self, Action moveAction, Action standardAction, TargetPath tPath)
    {
        // Move along the path
        foreach (var pos in tPath.path)
        {
            // If we can't move any further, end the turn
            if (!self.CanUseAction(moveAction))
                yield break;
            moveAction.UseAll(grid, self, pos);
            yield return new WaitForSeconds(moveDelay);
        }
        yield return StartCoroutine(AttackUntilExhausted(grid, self, standardAction, tPath.targetPos));
    }

    protected IEnumerable<Vector2Int> MovePositions(BattleGrid grid, Vector2Int pos, RangePattern pattern)
    {
        return pattern.GetPositions(grid, pos).Where((p) => grid.IsLegalAndEmpty(p));
    }

    protected Unit TargetPriority(Unit u1, Unit u2)
    {
        return u1.HP.CompareTo(u2.HP) <= 0 ? u1 : u2;
    }

    protected List<Vector2Int> Path(BattleGrid grid, T self, Action moveAction, Vector2Int goal, System.Predicate<Unit> canMoveThroughTarget = null)
    {
        var moveRange = moveAction.subActions[0].Range;
        // Calculate the maximum manhattan distance of the move action
        int maxMoveDist = moveRange.MaxDistance(grid);
        // Use the manhattan distance to the goal / the maximum manhattan distance of the move action as the heuristic
        float Heur(Vector2Int pos, Vector2Int goalPos) => ((int)Vector2Int.Distance(pos, goalPos)) / maxMoveDist;
        // No predicate, use basic adjacency
        if (canMoveThroughTarget == null)
        {
            // Use the move range and a legality / emptiness check for the adjacency function
            IEnumerable<Vector2Int> NodeAdj(Vector2Int p) => MovePositions(grid, p, moveRange);
            var path = Pathfinding.AStar.Pathfind(self.Pos, goal, NodeAdj, (p, pAdj) => 1, Heur);
            if (path != null)
            {
                // Remove the starting position from the path
                path.RemoveAt(0);
                return path;
            }
        }
        else // Predicate, use advanced adjacency
        {
            // Use the move range, a legality check, and an emptiness / predicate check for the adjacency function
            IEnumerable<Vector2Int> NodeAdj(Vector2Int p)
            {
                return moveRange.GetPositions(grid, p).Where((p2) => grid.IsLegal(p2) && (grid.IsEmpty(p2) || canMoveThroughTarget(grid.Get(p2))));
            }
            // Use 1 as the cost if the pos is empty, else use the number of grid positions (so paths will always prefer going through less things)
            int numGridPositions = grid.Dimensions.x * grid.Dimensions.y;
            float Cost(Vector2Int p, Vector2Int pAdj) => grid.IsEmpty(pAdj) ? 1 : numGridPositions;
            var path = Pathfinding.AStar.Pathfind(self.Pos, goal, NodeAdj, Cost, Heur);
            if (path != null)
            {
                // Remove the starting position from the path
                path.RemoveAt(0);
                return path;
            }
        }
        return null;
    }

    protected List<TargetPath> PathsToTargetRange(BattleGrid grid, T self, Action moveAction, Action standardAction, IEnumerable<Unit> targets, System.Predicate<Unit> canMoveThroughTarget = null)
    {
        var positions = new Dictionary<Vector2Int, Unit>();
        // Get relevant ranges
        var moveRange = moveAction.subActions[0].Range;
        var targetRange = standardAction.subActions[0].Range;
        // Target position is valid if is legal and is empty, self or passes the canMoveThrough pred
        bool ValidPos(Vector2Int p) => grid.IsLegal(p) && (grid.IsEmpty(p) || p == self.Pos
            || (canMoveThroughTarget != null && canMoveThroughTarget(grid.Get(p))));
        // Calculate possible target positions
        foreach (var target in targets)
        {
            var targetPositions = targetRange.GetPositions(grid, target.Pos).Where(ValidPos);
            // For now, just add the empty/self legal positions where a target would be in range
            foreach(var position in targetPositions)
            {
                if(!positions.ContainsKey(position))
                {
                    positions.Add(position, target);
                }
                else
                {
                    positions[position] = TargetPriority(positions[position], target);
                }
            }
        }
        // Find all shortests paths to target positions
        var paths = new List<TargetPath>();
        foreach (var posTargetPair in positions)
        {
            var path = Path(grid, self, moveAction, posTargetPair.Key, canMoveThroughTarget);
            if (path != null)
            {
                paths.Add(new TargetPath(path, posTargetPair.Value.Pos));
            }
        }
        // Move towards the target position with the shortest path
        paths.Sort((p1, p2) => p1.path.Count.CompareTo(p2.path.Count));
        return paths;
    }

    public List<Vector2Int> Reachable(BattleGrid grid, T self, Action moveAction, Vector2Int startPos)
    {
        int range = self.ActionUsesUntilNoAP(moveAction);
        // Use the move range and a legality / emptiness check for the adjacency function
        IEnumerable<Vector2Int> NodeAdj(Vector2Int p) => MovePositions(grid, p, moveAction.subActions[0].Range);
        // Initialize distances with the startPosition
        var distances = new Dictionary<Vector2Int, int> { { startPos, 0 } };

        // Inner recursive method
        void ReachableRecursive(Vector2Int p, int currDepth)
        {
            // Inner Traversability method
            bool Traversable(Vector2Int pos)
            {
                return (!distances.ContainsKey(pos) || currDepth + 1 < distances[pos]) && grid.IsLegalAndEmpty(pos);
            };

            // Log discovery and distance
            if (distances.ContainsKey(p))
            {
                if (distances[p] > currDepth)
                    distances[p] = currDepth;
            }
            else
            {
                distances.Add(p, currDepth);
            }
            // If depth is greater than range, end recursion
            if (currDepth >= range)
                return;
            // Get adjacent nodes (traversability function inverted as the Adj function takes a function for non-traversability)
            var nodes = NodeAdj(p).Where(Traversable);
            // Recur
            foreach (var node in nodes)
                ReachableRecursive(node, currDepth + 1);
        }

        // Start Recursion
        ReachableRecursive(startPos, 0);
        return distances.Keys.ToList();
    }

    /// <summary>
    /// Checks for tagets in range of the given standard action. Takes target pattern into account
    /// returns the first viable target position found, else BattleGrid.OutOfBounds
    /// </summary>
    protected Vector2Int CheckforTargets<Target>(BattleGrid grid, Unit self, Action standardAction, List<Target> targetUnits) where Target : Unit
    {
        var subAction = standardAction.subActions[0];
        foreach (var pos in subAction.Range.GetPositions(grid, self.Pos))
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

    public readonly struct TargetPath
    {
        public readonly List<Vector2Int> path;
        public readonly Vector2Int targetPos;

        public TargetPath(List<Vector2Int> path, Vector2Int targetPos)
        {
            this.path = path;
            this.targetPos = targetPos;
        }
    }
}
