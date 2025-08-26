using Extensions.VectorIntDimensionUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorAllUnitsOnTeams : RangePatternGenerator
{
    [SerializeField] private List<Unit.Team> targetTeams;
    [SerializeField] private bool includeSelf = true;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        foreach (var unit in grid)
        {
            if (targetTeams.Contains(unit.UnitTeam) && (includeSelf || unit != user))
            {
                yield return unit.Pos;
            }
        }
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        if (!grid.IsEmpty(targetPos))
            yield break;
        foreach (var pos in grid.Dimensions.Enumerate())
        {
            yield return pos;
        }
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.MaxGridDistance;
    }
}
