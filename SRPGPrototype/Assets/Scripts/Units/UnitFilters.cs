using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitFilters
{
    public static bool IsPlayer(Unit u) => u.UnitTeam == Unit.Team.Player;
}
