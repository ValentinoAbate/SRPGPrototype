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
public class AIComponentSpawner : AIComponent<AIUnit>
{
    protected virtual Action StandardAction => standardAction;
    [SerializeField] private Action standardAction;
    [SerializeField] private Animator animator;
    private static readonly int readyHash = Animator.StringToHash("Ready");

    public override IEnumerable<Action> Actions
    {
        get
        {
            yield return StandardAction;
        }
    }

    private bool skipTurn = false;

    public override void Initialize(AIUnit self)
    {
        standardAction = standardAction.Validate(self.ActionTransform);
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        skipTurn = !skipTurn;
        if(animator != null)
        {
            animator.SetBool(readyHash, skipTurn);
        }
        if (skipTurn)
        {
            yield break;
        }
        var subAction = StandardAction.SubActions[0];
        // If action targets self, end early
        if (subAction.targetPattern.patternType == TargetPattern.Type.Self)
        {
            yield return StartCoroutine(AttackUntilExhausted(grid, self, StandardAction, self.Pos));
            yield break;
        }
        // Check for targetspace in range
        while (!self.Dead && self.CanUseAction(StandardAction))
        {
            var tPos = CheckForEmptyTargetPosition(grid, self, StandardAction);
            if (tPos == BattleGrid.OutOfBounds)
                break;
            Debug.Log(self.DisplayName + " is targeting tile: " + tPos.ToString() + " for spawning!");
            yield return new WaitForSeconds(attackDelay);
            StandardAction.UseAll(grid, self, tPos);
        }
    }
}
