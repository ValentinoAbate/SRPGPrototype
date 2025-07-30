using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentPawn : AIComponent<AIUnit>
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
    [SerializeField] private GameObject promoteToUnitPrefab;

    public override void Initialize(AIUnit self)
    {
        standardAction = standardAction.Validate(self.ActionTransform);
        moveAction = moveAction.Validate(self.ActionTransform);
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        // Attack if able
        var attackRoutine = AttackBestPositionInRange(grid, self, standardAction, UnitFilters.IsPlayer);
        if (attackRoutine != null)
        {
            yield return attackRoutine;
        }
        // Try moving to any available space
        while (self.CanUseAction(moveAction))
        {
            bool foundMove = false;
            foreach (var rPos in moveAction.GetRange(grid, self.Pos, self))
            {
                if (grid.IsLegalAndEmpty(rPos))
                {
                    foundMove = true;
                    moveAction.UseAll(grid, self, rPos);
                    yield return moveDelay;
                    break;
                }
            }
            if (!foundMove)
            {
                break;
            }
        }
        // Promotion check
        if(self.Pos.y == 0)
        {
            // Cache position
            var pos = self.Pos;
            // Remove self from board
            grid.Remove(self);
            // Temporary: move to OOB
            transform.position = grid.GetSpace(BattleGrid.OutOfBounds);

            // Spawn Unit and add to the grid
            var unit = Instantiate(promoteToUnitPrefab).GetComponent<Unit>();
            grid.Add(pos, unit);
            unit.transform.position = grid.GetSpace(unit.Pos);
        }
    }
}
