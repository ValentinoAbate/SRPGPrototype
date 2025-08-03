using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectMove : ActionEffect
{
    public override bool IsValidTarget(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null && target.Movable;
    }
    /// <summary>
    /// A chained movement function. Returns true if the move was a success, else false.
    /// If unit exists in the destination square, tries to move it in the same direction (recursive).
    /// If any move in the chain fails, the whole move fails
    /// </summary>
    protected bool Move(BattleGrid grid, Unit target, Vector2Int direction)
    {
        if (!target.Movable)
            return false;
        Vector2Int destination = target.Pos + direction;
        if (!grid.IsLegal(destination))
            return false;
        var unitAtDestination = grid.Get(destination);
        if(unitAtDestination == null)
        {
            // Do move
            return EnactMove(grid, target, destination);
        }
        else if(Move(grid, unitAtDestination, direction))
        {
            // Do move
            return EnactMove(grid, target, destination);
        }
        return false;
    }

    protected bool SetPosition(BattleGrid grid, Unit target, Vector2Int position)
    {
        if (!target.Movable)
            return false;
        return EnactMove(grid, target, position);
    }

    /// <summary>
    /// Actually moves a unit, and shows any applicable visualizations
    /// </summary>
    private bool EnactMove(BattleGrid grid, Unit target, Vector2Int destination)
    {
        return grid.MoveAndSetWorldPos(target, destination);
    }
}
