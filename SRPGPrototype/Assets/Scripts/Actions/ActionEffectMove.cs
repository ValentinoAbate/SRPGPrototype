using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectMove : ActionEffect
{
    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null && target.Movable;
    }
    /// <summary>
    /// A chained movement function. Returns true if the move was a success, else false.
    /// If unit exists in the destination square, tries to move it in the same direction (recursive).
    /// If any move in the chain fails, the whole move fails
    /// </summary>
    protected bool Move(BattleGrid grid, Unit source, Unit target, Vector2Int direction, int distance = 1)
    {
        return target != null && TryMove(grid, source, target, direction, distance) > 0;
    }

    protected bool CanMove(BattleGrid grid, Unit source, Unit target, Vector2Int direction, int distance = 1)
    {
        if (target == null || !target.Movable)
        {
            return false;
        }
        var destination = target.Pos + direction;
        if (!grid.IsLegal(destination))
        {
            return false;
        }
        var blocker = grid.Get(destination);
        return blocker == null || CanMove(grid, source, blocker, direction, distance);
    }

    private int TryMove(BattleGrid grid, Unit source, Unit target, Vector2Int direction, int distance)
    {
        if (!target.Movable)
        {
            return 0;
        }
        for (int i = 0; i < distance; ++i)
        {
            var destination = target.Pos + (direction * (i + 1));
            if (!grid.IsLegal(destination))
            {
                return TryEnactMoveAtDistance(grid, source, target, direction, i);
            }
            var blocker = grid.Get(destination);
            if (blocker != null)
            {
                int blockerDistanceMoved = TryMove(grid, source, blocker, direction, distance - i);
                return TryEnactMoveAtDistance(grid, source, target, direction, i + blockerDistanceMoved);
            }
        }
        return TryEnactMoveAtDistance(grid, source, target, direction, distance);
    }

    private int TryEnactMoveAtDistance(BattleGrid grid, Unit source, Unit target, Vector2Int direction, int distance)
    {
        if(distance <= 0)
        {
            return 0;
        }
        return EnactMove(grid, source, target, target.Pos + (direction * distance)) ? distance : 0;
    }

    protected bool SetPosition(BattleGrid grid, Unit source, Unit target, Vector2Int position)
    {
        if (!target.Movable)
            return false;
        return EnactMove(grid, source, target, position);
    }

    protected bool Swap(BattleGrid grid, Unit source, Unit target1, Unit target2)
    {
        if (!target1.Movable && target2.Movable)
            return false;
        grid.SwapAndSetWorldPos(target1, target2);
        DoReposition(grid, source, target1);
        DoReposition(grid, source, target2);
        return true;
    }

    /// <summary>
    /// Actually moves a unit, and shows any applicable visualizations
    /// </summary>
    private bool EnactMove(BattleGrid grid, Unit source, Unit target, Vector2Int destination)
    {
        if(grid.MoveAndSetWorldPos(target, destination))
        {
            DoReposition(grid, source, target);
            return true;
        }
        return false;
    }

    private void DoReposition(BattleGrid grid, Unit source, Unit target)
    {
        source.OnRepositionOther?.Invoke(grid, source, target);
        target.OnRepositionedFn?.Invoke(grid, source, target);
    }
}
