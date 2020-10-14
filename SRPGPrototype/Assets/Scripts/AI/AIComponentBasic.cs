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
        // If action targets self, end early
        if (subAction.targetPattern.patternType == TargetPattern.Type.Self)
        {
            while(self.AP >= standardAction.APCost)
            {
                standardAction.UseAll(grid, self, self.Pos);
            }
            yield break;
        }
        // Find all targets
        var targetUnits = FindTargetsOnTeams(grid, targetTeams);
        // Exit early if there are no targets
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
                    standardAction.UseAll(grid, self, tPos);
                    yield return new WaitForSeconds(attackDelay);
                    Debug.Log(self.DisplayName + " is targeting tile: " + tPos.ToString() + " for an attack!");
                }
                else // No target is found, use move action
                {
                    // Exit if no targets and don't have enough AP
                    if (moveAction.APCost > self.AP)
                        yield break;
                    // Log the original position
                    Vector2Int oldPos = self.Pos;
                    yield return StartCoroutine(PathToTargetRange(grid, self, moveAction, standardAction, targetUnits));
                    // If we didn't actually move anywhere, end the turn
                    if (self.Pos == oldPos)
                        yield break;
                }
            }
            else // AP must be enough to use the move action
            {
                // Log the original position
                Vector2Int oldPos = self.Pos;
                yield return StartCoroutine(PathToTargetRange(grid, self, moveAction, standardAction, targetUnits));
                // If we didn't actually move anywhere, end the turn
                if (self.Pos == oldPos)
                    yield break;
            }
        }
    }
}
