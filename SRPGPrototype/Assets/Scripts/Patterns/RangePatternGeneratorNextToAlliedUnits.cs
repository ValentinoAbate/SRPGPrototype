using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RangePatternGeneratorNextToAlliedUnits : RangePatternGenerator
{ 
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        yield return userPos;
        if (user == null)
            yield break;
        foreach (var unit in grid) 
        { 
            if(unit.UnitTeam == user.UnitTeam && unit != user)
            {
                foreach(var pos in unit.Pos.Adjacent())
                {
                    if (grid.IsLegalAndEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }
    }

    public override IEnumerable<Vector2Int> ReverseGenerate(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        yield break; // not implementing unless needed
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.Dimensions.x + grid.Dimensions.y - 2;
    }
}
