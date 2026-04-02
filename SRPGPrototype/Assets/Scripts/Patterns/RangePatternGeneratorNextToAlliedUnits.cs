using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RangePatternGeneratorNextToAlliedUnits : RangePatternGeneratorNextToUnits
{
    protected override bool UnitPredicate(BattleGrid grid, Unit unit, Vector2Int userPos, Unit user)
    {
        return unit.UnitTeam == user.UnitTeam && unit != user;
    }
}
