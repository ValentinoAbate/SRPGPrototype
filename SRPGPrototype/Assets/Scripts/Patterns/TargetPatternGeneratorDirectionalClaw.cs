using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorDirectionalClaw : TargetPatternGenerator
{
    [SerializeField] private int distance = 1;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        var direction = user.Pos.DirectionTo(targetPos);
        for (int i = 1; i <= distance; i++)
        {
            yield return targetPos - new Vector2Int(direction.x * distance, 0);
            yield return targetPos - new Vector2Int(0, direction.y * distance);
        }
        yield return user.Pos - direction;
    }
}
