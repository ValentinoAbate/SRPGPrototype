using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorAllUnitsOnTeams : TargetPatternGenerator
{
    [SerializeField] private List<Unit.Team> targetTeams;
    [SerializeField] private bool includeSelf = true;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        foreach(var unit in grid)
        {
            if ((targetTeams.Count <= 0 || targetTeams.Contains(unit.UnitTeam)) && IsTargetValid(grid, user, unit, targetPos) && (includeSelf || unit != user))
            {
                yield return unit.Pos;
            }
        }
    }

    protected virtual bool IsTargetValid(BattleGrid grid, Unit user, Unit target, Vector2Int targetPos) => true;
}
