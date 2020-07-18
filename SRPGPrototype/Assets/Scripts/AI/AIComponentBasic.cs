using JetBrains.Annotations;
using System;
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
public class AIComponentBasic : AIComponent<EnemyUnit>
{
    public Action moveAction;
    public Action standardAction;

    public override List<Action> Actions => new List<Action> { moveAction, standardAction };

    public override IEnumerator DoTurn(BattleGrid grid, EnemyUnit self)
    {
        var subAction = standardAction.subActions[0];
        var playerUnits = grid.FindAll<PlayerUnit>();
        if (playerUnits.Count <= 0)
            yield break;
        if (subAction.targetPattern.patternType == TargetPattern.Type.Self)
        {
            while(self.AP >= standardAction.APCost)
            {
                self.AP -= standardAction.APCost;
                subAction.Use(grid, self, self.Pos);
            }
            yield break;
        }
        while (self.AP >= standardAction.APCost || self.AP >= moveAction.APCost)
        {
            if (self.AP >= standardAction.APCost)
            {
                bool foundTarget = false;
                foreach (var pos in subAction.range.OffsetsShifted(self.Pos - subAction.range.Center))
                {
                    var targetPositions = subAction.targetPattern.Target(grid, self, pos);
                    foreach (var tPos in targetPositions)
                    {
                        if (playerUnits.Any((u) => u.Pos == tPos))
                        {
                            standardAction.Use(self);
                            subAction.Use(grid, self, pos);
                            yield return new WaitForSeconds(0.5f);
                            Debug.Log(self.DisplayName + " is targeting tile: " + pos.ToString() + " for an attack!");
                            foundTarget = true;
                            break;
                        }
                    }
                    if (foundTarget)
                        break;
                }
                if (!foundTarget) // No target is found, use move action
                {
                    // Exit if no targets and don't have enough AP
                    if (moveAction.APCost > self.AP)
                        yield break;
                    yield return StartCoroutine(MoveToTargetRange(grid, self, playerUnits));
                }
            }
            else // AP must be enough to use the move action
            {
                yield return StartCoroutine(MoveToTargetRange(grid, self, playerUnits));
            }
        }
    }

    private IEnumerator MoveToTargetRange(BattleGrid grid, Combatant self, List<PlayerUnit> playerUnits)
    {
        var sub = moveAction.subActions[0];
        var movePositions = sub.range.OffsetsShifted(self.Pos - sub.range.Center)
            .Where((p) =>  grid.IsLegal(p) && grid.IsEmpty(p));
        // Move into a viable attack position if possible
        foreach (var pos in movePositions)
        {
            // If any of the target positions targeting this square contain a player unit
            if(sub.targetPattern.Target(grid, self, pos).Any((p) => playerUnits.Any((playerUnit) => p == playerUnit.Pos)))
            {
                moveAction.Use(self);
                sub.Use(grid, self, pos);
                yield return new WaitForSeconds(moveDelay);
                yield break;
            }
        }
        var posByPathDist = movePositions.Select((p) => new Tuple<Vector2Int, int>(p, playerUnits.Min((unit) => GetPathDist(grid, p, unit.Pos))));
        // Else find the position that can be moved to that brings us closest to a player unit
        var closestPos = posByPathDist.OrderBy((t) => t.Item2).First().Item1;
        moveAction.Use(self);
        sub.Use(grid, self, closestPos);
        yield return new WaitForSeconds(moveDelay);
    }
    private int GetPathDist(BattleGrid grid, Vector2Int start, Vector2Int goal)
    {
        var path = grid.Path(start, goal, (p) => p == null || p.Pos == start || p.Pos == goal);
        if (path == null)
            return int.MaxValue;
        return path.Count;
    }
}
