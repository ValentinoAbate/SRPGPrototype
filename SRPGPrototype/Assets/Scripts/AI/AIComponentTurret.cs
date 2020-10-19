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
public class AIComponentTurret : AIComponent<AIUnit>
{
    public List<Unit.Team> targetTeams = new List<Unit.Team> { Unit.Team.Player };
    [SerializeField]
    private Action standardAction;

    public override List<Action> Actions => new List<Action> { standardAction };

    public override void Initialize(AIUnit self)
    {
        standardAction = standardAction.Validate(self.ActionTransform);
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        var subAction = standardAction.subActions[0];
        // If action targets self, end early
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
        }
    }
}
