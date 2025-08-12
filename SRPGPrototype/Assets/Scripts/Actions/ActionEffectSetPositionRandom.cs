using RandomUtils;
using System.Linq;

public class ActionEffectSetPositionRandom : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        var emptySpaces = grid.EmptyPositions;
        if (!emptySpaces.Any())
            return;
        grid.MoveAndSetWorldPos(target, RandomU.instance.Choice(emptySpaces));
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return target != null && target.Movable && grid.EmptyPositions.Any();
    }
}
