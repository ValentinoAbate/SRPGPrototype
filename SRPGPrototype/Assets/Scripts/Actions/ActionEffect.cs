using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffect : MonoBehaviour
{
    public virtual void Initialize(BattleGrid grid, Combatant user, List<Vector2Int> targetPositions) { }
    public abstract void ApplyEffect(BattleGrid grid, Combatant user, PositionData targetData);

    public struct PositionData
    {
        // The position of the current target tile
        public Vector2Int targetPos;
        // The position that was originally selected as the target for the action
        public Vector2Int selectedPos;

        public PositionData(Vector2Int target, Vector2Int selected)
        {
            targetPos = target;
            selectedPos = selected;
        }
    }

}
