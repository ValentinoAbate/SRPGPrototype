using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AIComponent<T> : MonoBehaviour where T : AIUnit
{
    // amount of time to pause for each square moved
    private const float moveDelayTime = 0.25f;
    private const float attackDelayTime = 0.2f;

    protected static readonly WaitForSeconds moveDelay = new WaitForSeconds(moveDelayTime);
    protected static readonly WaitForSeconds attackDelay = new WaitForSeconds(attackDelayTime);

    public abstract IEnumerable<Action> Actions { get; }

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
            yield return moveDelay;
        }
    }

    protected Coroutine RunAway(BattleGrid grid, T self, Action moveAction, System.Predicate<Unit> runAwayFrom, int apToSave = 0)
    {
        var reachable = Reachable(grid, self, moveAction, self.Pos, apToSave);
        if (reachable.Count == 0)
            return null;
        var runAwayFromUnits = grid.FindAll(runAwayFrom);
        float highestScore = 0;
        Vector2Int bestPos = self.Pos;
        foreach(var kvp in reachable)
        {
            float score = ComputeRunAwayScore(kvp.Key, runAwayFromUnits);
            if(score > highestScore)
            {
                highestScore = score;
                bestPos = kvp.Key;
            }
        }
        if (bestPos == self.Pos)
            return null;
        var path = Path(grid, self, moveAction, bestPos);
        return StartCoroutine(MoveAlongPath(grid, self, moveAction, path));
    }

    private float ComputeRunAwayScore(Vector2Int pos, IReadOnlyList<Unit> runAwayFrom)
    {
        float score = 0;
        foreach(var unit in runAwayFrom)
        {
            score += Vector2Int.Distance(pos, unit.Pos);
        }
        return score;
    }

    protected IEnumerator AttackUntilExhausted(BattleGrid grid, T self, Action standardAction, Vector2Int tPos)
    {
        while (!self.Dead && self.CanUseAction(standardAction))
        {
            Debug.Log(self.DisplayName + " is targeting tile: " + tPos.ToString() + " for an attack!");
            yield return attackDelay;
            standardAction.UseAll(grid, self, tPos);
        }
    }

    protected Coroutine AttackFirstUnitInRange(BattleGrid grid, T self, Action standardAction, System.Predicate<Unit> isUnitTarget)
    {
        // Find all targets
        var targetUnits = grid.FindAll(isUnitTarget);
        // Exit early if there are no targets
        if (targetUnits.Count <= 0)
            return null;
        // Check for target in range
        var tPos = GetFirstValidTargetPosInRange(grid, self, standardAction, targetUnits);
        // Use standard action until exhausted if target is found, then end turn
        if (tPos != BattleGrid.OutOfBounds)
        {
            return StartCoroutine(AttackUntilExhausted(grid, self, standardAction, tPos));
        }
        return null;
    }

    protected Coroutine AttackBestPositionInRange(BattleGrid grid, T self, Action standardAction, System.Predicate<Unit> isUnitTarget)
    {
        // Check for target in range
        var tPos = GetBestValidTargetPosInRange(grid, self, standardAction, isUnitTarget);
        // Use standard action until exhausted if target is found, then end turn
        if (tPos != BattleGrid.OutOfBounds)
        {
            return StartCoroutine(AttackUntilExhausted(grid, self, standardAction, tPos));
        }
        return null;
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
            yield return moveDelay;
        }
        yield return StartCoroutine(AttackUntilExhausted(grid, self, standardAction, tPath.targetPos));
    }

    protected IEnumerable<Vector2Int> MovePositions(BattleGrid grid, Vector2Int pos, RangePattern pattern)
    {
        return pattern.GetPositions(grid, pos, null).Where((p) => grid.IsLegalAndEmpty(p));
    }

    protected Unit TargetPriority(Unit u1, Unit u2)
    {
        return u1.HP.CompareTo(u2.HP) <= 0 ? u1 : u2;
    }

    protected List<Vector2Int> Path(BattleGrid grid, T self, Action moveAction, Vector2Int goal, System.Predicate<Unit> canMoveThroughTarget = null)
    {
        var moveRange = moveAction.SubActions[0].Range;
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
                return moveRange.GetPositions(grid, p, null).Where((p2) => grid.IsLegal(p2) && (grid.IsEmpty(p2) || canMoveThroughTarget(grid.Get(p2))));
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
        var positions = new Dictionary<TargetData, Unit>();
        // Get relevant ranges
        var moveRange = moveAction.SubActions[0].Range;
        var targetRange = standardAction.SubActions[0].Range;
        var targetPattern = standardAction.SubActions[0].targetPattern;
        // Target position is valid if is legal and is empty, self or passes the canMoveThrough pred
        bool ValidPos(Vector2Int p)
        {
            return grid.IsLegal(p) && (grid.IsEmpty(p) || p == self.Pos || (canMoveThroughTarget != null && canMoveThroughTarget(grid.Get(p))));
        }

        // Calculate possible target positions
        foreach (var target in targets)
        {
            foreach(var potentialTargetPos in targetPattern.ReverseTarget(grid, target.Pos))
            {
                foreach(var potentialUnitPos in targetRange.ReverseRange(grid, potentialTargetPos, self))
                {
                    if (!ValidPos(potentialUnitPos))
                    {
                        continue;
                    }
                    var posData = new TargetData(potentialUnitPos, potentialTargetPos);
                    if (!positions.ContainsKey(posData))
                    {
                        positions.Add(posData, target);
                    }
                    else
                    {
                        positions[posData] = TargetPriority(positions[posData], target);
                    }
                }
            }
        }
        // Find all shortests paths to target positions
        var paths = new List<TargetPath>(positions.Count);
        foreach (var posTargetPair in positions)
        {
            var path = Path(grid, self, moveAction, posTargetPair.Key.userPos, canMoveThroughTarget);
            if (path != null)
            {
                paths.Add(new TargetPath(path, posTargetPair.Key.targetPos));
            }
        }
        // Move towards the target position with the shortest path
        paths.Sort(ShortestTargetPathComparer);
        return paths;
    }

    public Dictionary<Vector2Int, int> Reachable(BattleGrid grid, T self, Action moveAction, Vector2Int startPos, int apToSave = 0)
    {
        int range = self.ActionUsesUntilNoAP(moveAction, apToSave);
        return Reachable(grid, moveAction, startPos, range);
    }

    public Dictionary<Vector2Int, int> Reachable(BattleGrid grid, Action moveAction, Vector2Int startPos, int range)
    {
        // Use the move range and a legality / emptiness check for the adjacency function
        IEnumerable<Vector2Int> NodeAdj(Vector2Int p) => MovePositions(grid, p, moveAction.SubActions[0].Range);
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
        return distances;
    }

    /// <summary>
    /// Checks for positions in range of the given standard action that will hit at least one target unit
    /// returns the first viable target position found, else BattleGrid.OutOfBounds
    /// </summary>
    protected Vector2Int GetFirstValidTargetPosInRange<Target>(BattleGrid grid, Unit self, Action standardAction, IEnumerable<Target> targetUnits) where Target : Unit
    {
        foreach (var pos in standardAction.GetRange(grid, self.Pos, self))
        {
            foreach (var tPos in standardAction.GetTargets(grid, self, pos))
            {
                foreach(var target in targetUnits)
                {
                    if (target.Pos == tPos)
                        return pos;
                }
            }
        }
        return BattleGrid.OutOfBounds;
    }

    protected Vector2Int GetBestValidTargetPosInRange(BattleGrid grid, Unit self, Action standardAction, System.Predicate<Unit> isTarget)
    {
        int highestScore = 0;
        Vector2Int bestPos = BattleGrid.OutOfBounds;
        foreach (var pos in standardAction.GetRange(grid, self.Pos, self))
        {
            int score = 0;
            foreach (var tPos in standardAction.GetTargets(grid, self, pos))
            {
                var unit = grid.Get(tPos);
                if (unit == null)
                    continue;
                if (isTarget(unit))
                {
                    score += 200 - System.Math.Min(190, unit.HP);
                }
                else if(unit == self)
                {
                    score -= 5;
                }
            }
            if(score > highestScore)
            {
                highestScore = score;
                bestPos = pos;
            }
        }
        return bestPos;
    }

    protected Vector2Int CheckForEmptyTargetPosition(BattleGrid grid, Unit self, Action standardAction)
    {
        foreach (var pos in standardAction.GetRange(grid, self.Pos, self))
        {
            foreach (var tPos in standardAction.GetTargets(grid, self, pos))
            {
                if (grid.IsLegalAndEmpty(tPos))
                    return pos;
            }
        }
        return BattleGrid.OutOfBounds;
    }

    protected Vector2Int ChooseRandomEmptyTargetPosition(BattleGrid grid, Unit self, Action standardAction)
    {
        var positions = new List<Vector2Int>();
        foreach (var pos in standardAction.GetRange(grid, self.Pos, self))
        {
            foreach (var tPos in standardAction.GetTargets(grid, self, pos))
            {
                if (grid.IsLegalAndEmpty(tPos))
                {
                    positions.Add(pos);
                    break;
                }
            }
        }
        return positions.Count > 0 ? RandomU.instance.Choice(positions) : BattleGrid.OutOfBounds;
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

    public readonly struct TargetData
    {
        public readonly Vector2Int userPos;
        public readonly Vector2Int targetPos;

        public TargetData(Vector2Int userPos, Vector2Int targetPos)
        {
            this.userPos = userPos;
            this.targetPos = targetPos;
        }
    }

    protected static int ShortestPathComparer(IReadOnlyCollection<Vector2Int> path1, IReadOnlyCollection<Vector2Int> path2)
    {
        return path1.Count.CompareTo(path2.Count);
    }

    private static int ShortestTargetPathComparer(TargetPath p1, TargetPath p2) => p1.path.Count.CompareTo(p2.path.Count);
}
