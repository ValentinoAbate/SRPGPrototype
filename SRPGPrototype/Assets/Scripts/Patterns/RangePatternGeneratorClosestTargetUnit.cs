using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RangePatternGeneratorClosestTargetUnit : RangePatternGenerator
{
    [SerializeField] private List<Unit.Team> targetTeams;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        float closestDistance = float.MaxValue;
        Unit target = null;
        foreach(var unit in grid.FindAll(IsTarget))
        {
            float distance = Vector2Int.Distance(userPos, unit.Pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = unit;
            }
        }
        if(target != null)
        {
            yield return target.Pos;
        }
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        yield break; // Not implementing unless needed
    }

    private bool IsTarget(Unit unit) => targetTeams.Contains(unit.UnitTeam);

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.MaxGridDistance;
    }
}
