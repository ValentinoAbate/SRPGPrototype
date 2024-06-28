using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A basic AI with a move action and a standard action.
/// uses the standard action if in range, else tries to move into range or close to in range.
/// Only works with single-action actions currently.
/// Assumes that the unit will move to any tile targeted by a move action
/// </summary>
public class AIComponentBasic : AIComponent<AIUnit>
{
    public List<Unit.Team> targetTeams = new List<Unit.Team> { Unit.Team.Player };
    [SerializeField]
    private Action moveAction;
    [SerializeField]
    private Action standardAction;

    public override List<Action> Actions => new List<Action> { moveAction, standardAction };

    public override void Initialize(AIUnit self)
    {
        moveAction = moveAction.Validate(self.ActionTransform);
        standardAction = standardAction.Validate(self.ActionTransform);
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        var subAction = standardAction.SubActions[0];
        // If action targets self, use it on self end early
        if (subAction.targetPattern.patternType == TargetPattern.Type.Self)
        {
            yield return StartCoroutine(AttackUntilExhausted(grid, self, standardAction, self.Pos));
            yield break;
        }
        // Find all targets
        var targetUnits = grid.FindAll(IsUnitTarget);
        // Exit early if there are no targets
        if (targetUnits.Count <= 0)
            yield break;
        // Check for target in range
        var tPos = CheckforTargets(grid, self, standardAction, targetUnits);
        // Use standard action until exhausted if target is found, then end turn
        if (tPos != BattleGrid.OutOfBounds)
        {
            yield return StartCoroutine(AttackUntilExhausted(grid, self, standardAction, tPos));
            yield break;
        }
        // Attempt to move into range
        // If we don't have enough AP to move, end early
        if (!self.CanUseAction(moveAction))
            yield break;
        // If there are no available moves (all moves are illegal or blocked), end early
        if (MovePositions(grid, self.Pos, moveAction.SubActions[0].Range).Count() <= 0)
            yield break;
        // Find Paths to target range
        var paths = PathsToTargetRange(grid, self, moveAction, standardAction, targetUnits);
        // If a path was found, take it
        if(paths.Count > 0)
        {
            yield return StartCoroutine(PathThenAttackIfAble(grid, self, moveAction, standardAction, paths[0]));
            yield break;
        }
        // Predicate for determining if a unit is an ally
        bool IsUnitAlly(Unit other) => other.UnitTeam == self.UnitTeam;
        // (If allies exist) Get paths to target range through allies
        var allyUnits = grid.FindAll(IsUnitAlly);
        if(allyUnits.Count > 0)
        {
            paths = PathsToTargetRange(grid, self, moveAction, standardAction, targetUnits, IsUnitAlly);
            // If path to target range through allies exists
            if(paths.Count > 0)
            {
                var shortenedPathsByTargetDist = new List<PathWithValue>();
                // Get a pair of final square that could be moved to (counting allies as obstacles) and path distance to target (moves left in path)
                foreach(var tPath in paths)
                {
                    var path = tPath.path;
                    // Get the path up unil the point where an ally blocks it (possibly add AP cost as a factor)
                    var shortenedPath = path.TakeWhile(grid.IsEmpty).ToList();
                    int targetDist = path.Count - shortenedPath.Count;
                    shortenedPathsByTargetDist.Add(new PathWithValue(shortenedPath, targetDist));
                }
                // Sort to find the lowest target path distance
                shortenedPathsByTargetDist.Sort((p1, p2) => p1.value.CompareTo(p2.value));
                // Move along the path
                yield return StartCoroutine(MoveAlongPath(grid, self, moveAction, shortenedPathsByTargetDist[0].path));
                // End the turn
                yield break;
            }
        }
        // Predicate for determining if unit is an obstacle
        bool IsUnitObstacle(Unit other) => !IsUnitAlly(other) && !IsUnitTarget(other);
        var obstacleUnits = grid.FindAll(IsUnitObstacle);
        // (If obstacles exist) Get path to target range through obstacles
        if (obstacleUnits.Count > 0)
        {
            paths = PathsToTargetRange(grid, self, moveAction, standardAction, targetUnits, IsUnitObstacle);
            // If path to target range through obstacles exists
            if (paths.Count > 0)
            {
                // Get target paths with number of obstacles they contain cached
                var pathsByObjNum = paths.Select((tPath) => new TPathWithValue(tPath, tPath.path.Count(grid.NotEmpty))).ToList();
                static int PathCompare(TPathWithValue p1, TPathWithValue p2)
                {
                    int numObstaclesComp = p1.value.CompareTo(p2.value);
                    if (numObstaclesComp != 0)
                        return numObstaclesComp;
                    return p1.tPath.path.Count.CompareTo(p2.tPath.path.Count);
                }
                // Sort by num obstacles, path length if equal
                pathsByObjNum.Sort(PathCompare);
                // Find an obstacle blocking the path to the attack
                foreach(var tPathWithValue in pathsByObjNum)
                {
                    var targetObstacles = tPathWithValue.tPath.path.Where(grid.NotEmpty).Select(grid.Get);
                    paths = PathsToTargetRange(grid, self, moveAction, standardAction, targetObstacles);
                    if(paths.Count > 0)
                    {
                        yield return StartCoroutine(PathThenAttackIfAble(grid, self, moveAction, standardAction, paths[0]));
                        yield break;
                    }
                }
            }
        }
        // Else no meaningful path to target range exists, try and path to a target
        // Find all shortest paths to targets
        var pathsToPositions = new List<List<Vector2Int>>(targetUnits.Count);
        foreach (var targetUnit in targetUnits)
        {
            bool CanPassThroughTarget(Unit u)
            {
                return u == targetUnit || IsUnitAlly(u);
            }
            var path = Path(grid, self, moveAction, targetUnit.Pos, CanPassThroughTarget);
            if (path != null)
            {
                var shortenedPath = path.TakeWhile(grid.IsEmpty).ToList();
                pathsToPositions.Add(shortenedPath);
            }
        }
        if (pathsToPositions.Count > 0)
        {
            pathsToPositions.Sort(ShortestPathComparer);
            if (pathsToPositions[0].Count > 0)
            {
                // Move along the path
                yield return MoveAlongPath(grid, self, moveAction, pathsToPositions[0]);
            }
            else
            {
                // Already in optimal position, do nothing
                yield break;
            }
        }
        // Else no meaningful path to target exists, just try and get close to a tile adjacent to a target
        // Find all shortest paths to positions adjacent to targets
        pathsToPositions.Clear();
        var attackRangeType = standardAction.SubActions[0].Range.patternType;
        // If attack range type is adjacent both, then we already tried this, break
        if (attackRangeType != RangePattern.Type.AdjacentBoth)
        {
            var positionsToCheck = new List<Vector2Int>(attackRangeType == RangePattern.Type.Adjacent || attackRangeType == RangePattern.Type.AdjacentDiagonal ? 4 : 8);
            foreach (var targetUnit in targetUnits)
            {
                positionsToCheck.Clear();
                if (attackRangeType != RangePattern.Type.Adjacent)
                {
                    targetUnit.Pos.AddAdjacent(positionsToCheck);
                }
                if (attackRangeType != RangePattern.Type.AdjacentDiagonal)
                {
                    targetUnit.Pos.AddAdjacentDiagonal(positionsToCheck);
                }
                foreach (var closePos in positionsToCheck)
                {
                    var path = Path(grid, self, moveAction, closePos, IsUnitAlly);
                    if (path != null)
                    {
                        var shortenedPath = path.TakeWhile(grid.IsEmpty).ToList();
                        pathsToPositions.Add(shortenedPath);
                    }
                }
            }
            if (pathsToPositions.Count > 0)
            {
                pathsToPositions.Sort(ShortestPathComparer);
                if (pathsToPositions[0].Count > 0)
                {
                    // Move along the path
                    yield return MoveAlongPath(grid, self, moveAction, pathsToPositions[0]);
                }
            }
        }

        // No viable path, attempt to simply get closer as a last-ditch effort
        int DistanceToClosestTarget(Vector2Int p)
        {
            int minDist = int.MaxValue;
            foreach(var targetUnit in targetUnits)
            {
                int dist = p.GridDistance(targetUnit.Pos);
                if(dist < minDist)
                {
                    minDist = dist;
                }
            }
            return minDist;
        }
        // Get reachable positions with target manhattan distance
        var reachablePosData = Reachable(grid, self, moveAction, self.Pos);
        var reachablePositions = new List<ReachablePositionData>(reachablePosData.Count);
        foreach(var reachablePosDatum in reachablePosData)
        {
            reachablePositions.Add(new ReachablePositionData(reachablePosDatum.Key, DistanceToClosestTarget(reachablePosDatum.Key), reachablePosDatum.Value));
        }
        reachablePositions.Sort();
        // If we can get closer, do so
        if (reachablePositions[0].pos != self.Pos)
        {
            var pathToClosePos = Path(grid, self, moveAction, reachablePositions[0].pos);
            yield return StartCoroutine(MoveAlongPath(grid, self, moveAction, pathToClosePos));
        }

        // Nothing to be done
        yield break;
    }

    private bool IsUnitTarget(Unit other) => targetTeams.Contains(other.UnitTeam);

    private readonly struct ReachablePositionData : System.IComparable<ReachablePositionData>
    {
        public readonly Vector2Int pos;
        public readonly float distanceToClosestTarget;
        public readonly int movesToReach;

        public ReachablePositionData(Vector2Int pos, float distanceToClosestTarget, int movesToReach)
        {
            this.pos = pos;
            this.distanceToClosestTarget = distanceToClosestTarget;
            this.movesToReach = movesToReach;
        }

        public int CompareTo(ReachablePositionData other)
        {
            return distanceToClosestTarget.CompareTo(other.distanceToClosestTarget);
        }
    }

    private readonly struct PathWithValue
    {
        public readonly List<Vector2Int> path;
        public readonly int value;

        public PathWithValue(List<Vector2Int> path, int value)
        {
            this.path = path;
            this.value = value;
        }
    }

    private readonly struct TPathWithValue
    {
        public readonly TargetPath tPath;
        public readonly int value;

        public TPathWithValue(in TargetPath tPath, int value)
        {
            this.tPath = tPath;
            this.value = value;
        }
    }
}
