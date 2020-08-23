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
public class AIComponentBasic : AIComponent<Unit>
{
    public List<Unit.Team> targetTeams = new List<Unit.Team> { Unit.Team.Player };
    public Action moveAction;
    public Action standardAction;

    public override List<Action> Actions => new List<Action> { moveAction, standardAction };

    public override IEnumerator DoTurn(BattleGrid grid, Unit self)
    {
        var subAction = standardAction.subActions[0];
        var targetUnits = FindTargetsOnTeams(grid, targetTeams);

        if (subAction.targetPattern.patternType == TargetPattern.Type.Self)
        {
            while(self.AP >= standardAction.APCost)
            {
                standardAction.StartAction(self);
                subAction.Use(grid, standardAction, self, self.Pos);
                standardAction.FinishAction(self);
            }
            yield break;
        }
        if (targetUnits.Count <= 0)
            yield break;
        while (self.AP >= standardAction.APCost || self.AP >= moveAction.APCost)
        {
            if (self.AP >= standardAction.APCost)
            {
                var tPos = CheckforTargets(grid, self, standardAction, targetUnits);
                // Use standard action if target is found
                if(tPos != BattleGrid.OutOfBounds)
                {
                    standardAction.StartAction(self);
                    subAction.Use(grid, standardAction, self, tPos);
                    standardAction.FinishAction(self);
                    yield return new WaitForSeconds(attackDelay);
                    Debug.Log(self.DisplayName + " is targeting tile: " + tPos.ToString() + " for an attack!");
                }
                else // No target is found, use move action
                {
                    // Get move positions
                    var movePositions = moveAction.subActions[0].range.OffsetsShifted(self.Pos)
                        .Where((p) => grid.IsLegal(p) && grid.IsEmpty(p));
                    // Break if nowhere to move
                    if (movePositions.Count() <= 0)
                        yield break;
                    // Exit if no targets and don't have enough AP
                    if (moveAction.APCost > self.AP)
                        yield break;
                    yield return StartCoroutine(MoveToTargetRange(grid, self, moveAction, targetUnits));
                }
            }
            else // AP must be enough to use the move action
            {
                // Get move positions
                var movePositions = moveAction.subActions[0].range.OffsetsShifted(self.Pos)
                    .Where((p) => grid.IsLegal(p) && grid.IsEmpty(p));
                // Break if nowhere to move
                if (movePositions.Count() <= 0)
                    yield break;
                yield return StartCoroutine(MoveToTargetRange(grid, self, moveAction, targetUnits));
            }
        }
    }
}
