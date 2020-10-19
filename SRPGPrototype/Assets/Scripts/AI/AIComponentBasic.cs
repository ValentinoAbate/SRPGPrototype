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
        var subAction = standardAction.subActions[0];
        // If action targets self, use it on self end early
        if (subAction.targetPattern.patternType == TargetPattern.Type.Self)
        {
            yield return StartCoroutine(AttackUntilExhausted(grid, self, standardAction, self.Pos));
            yield break;
        }
        bool IsUnitTarget(Unit other) => targetTeams.Contains(other.UnitTeam);
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
        if (MovePositions(grid, self.Pos, moveAction.subActions[0].Range).Count() <= 0)
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
                int PathCompare(TPathWithValue p1, TPathWithValue p2)
                {
                    int numObstaclesComp = p1.value.CompareTo(p2.value);
                    if (numObstaclesComp != 0)
                        return numObstaclesComp;
                    return p1.tPath.path.Count.CompareTo(p2.tPath.path.Count);
                }
                // Sort by num obstacles, path length if equal
                pathsByObjNum.Sort(PathCompare);
                // Find an obstacle blocking the path the attack
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
                yield break;
            }
        }
        // Else no meaningful path to target range exists, just try and get close to a target
        PosWithValue PosWithTargetRange(Vector2Int p) => new PosWithValue(p, targetUnits.Min((u) => (int)Vector2Int.Distance(u.Pos, p)));
        // Get reachable positions with target manhattan distance
        var positions = Reachable(grid, self, moveAction, self.Pos).Select(PosWithTargetRange).ToList();
        positions.Sort((p1, p2) => p1.value.CompareTo(p2.value));
        // Already in the best spot, end turn
        if (positions[0].pos == self.Pos)
            yield break;
        var pathToClosePos = Path(grid, self, moveAction, positions[0].pos);
        yield return StartCoroutine(MoveAlongPath(grid, self, moveAction, pathToClosePos));
    }

    private readonly struct PosWithValue
    {
        public readonly Vector2Int pos;
        public readonly int value;

        public PosWithValue(Vector2Int pos, int value)
        {
            this.pos = pos;
            this.value = value;
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
