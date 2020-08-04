using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectMove : ActionEffect
{
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
            EnactMove(grid, target, destination);
            return true;
        }
        else if(Move(grid, unitAtDestination, direction))
        {
            // Do move
            EnactMove(grid, target, destination);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Actually moves a unit, and shows any applicable visualizations
    /// </summary>
    private void EnactMove(BattleGrid grid, Unit target, Vector2Int destination)
    {
        grid.MoveAndSetWorldPos(target, destination);
    }
}
