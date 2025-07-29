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
        var attackRoutine = AttackBestPositionInRange(grid, self, standardAction, UnitFilters.IsPlayer);
        if (attackRoutine != null)
        {
            yield return attackRoutine;
        }
        var path = Path(grid, self, moveAction, new Vector2Int(self.Pos.x, 0));
        if(path != null)
        {
            yield return StartCoroutine(MoveAlongPath(grid, self, moveAction, path));
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
