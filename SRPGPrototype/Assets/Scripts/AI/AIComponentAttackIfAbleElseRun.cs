using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentAttackIfAbleElseRun : AIComponent
{
    public override IEnumerable<Action> Actions
    {
        get
        {
            yield return standardAction;
            yield return moveAction;
        }
    }

    [SerializeField] private Action standardAction;
    [SerializeField] private Action moveAction;

    public override void Initialize(AIUnit self)
    {
        standardAction = standardAction.Validate(self.ActionTransform);
        moveAction = moveAction.Validate(self.ActionTransform);
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        var attackRoutine = AttackBestPositionInRange(grid, self, standardAction, UnitFilters.IsPlayer);
        if(attackRoutine != null)
        {
            yield return attackRoutine;
        }
        var runAwayRoutine = RunAway(grid, self, moveAction, UnitFilters.IsPlayer);
        if (runAwayRoutine != null)
        {
            yield return runAwayRoutine;
        }
    }


}
