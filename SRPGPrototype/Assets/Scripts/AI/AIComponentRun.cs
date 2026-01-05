using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentRun : AIComponent<AIUnit>
{
    public override IEnumerable<Action> Actions
    {
        get
        {
            yield return moveAction;
        }
    }

    [SerializeField] private Action moveAction;

    public override void Initialize(AIUnit self)
    {
        moveAction = moveAction.Validate(self.ActionTransform);
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        var runAwayRoutine = RunAway(grid, self, moveAction, UnitFilters.IsPlayer);
        if (runAwayRoutine != null)
        {
            yield return runAwayRoutine;
        }
    }


}
