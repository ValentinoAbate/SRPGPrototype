using RandomUtils;

public class ActionEffectSetPositionRandom : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if (target == null)
            return;
        var emptySpaces = grid.EmptyPositions;
        if (emptySpaces.Count <= 0)
            return;
        grid.MoveAndSetWorldPos(target, RandomU.instance.Choice(emptySpaces));
    }
}
