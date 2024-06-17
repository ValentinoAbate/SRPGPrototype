using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RangePatternGeneratorNextToAlliedUnits : RangePatternGenerator
{ 
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        var positions = new List<Vector2Int>() { userPos };
        if (user == null)
            return positions;
        foreach (var unit in grid) 
        { 
            if(unit.UnitTeam == user.UnitTeam && unit != user)
            {
                foreach(var pos in unit.Pos.Adjacent())
                {
                    if (grid.IsLegalAndEmpty(pos))
                    {
                        positions.Add(pos);
                    }
                }
            }
        }
        return positions;
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return grid.Dimensions.x + grid.Dimensions.y - 2;
    }
}
