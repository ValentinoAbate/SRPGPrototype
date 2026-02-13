using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitFilters
{
    public static bool IsPlayer(Unit u) => u.UnitTeam == Unit.Team.Player;
    public static bool IsOnTeam(Unit u, params Unit.Team[] teams)
    {
        foreach(var team in teams)
        {
            if(u.UnitTeam == team)
            {
                return true;
            }
        }
        return false;
    }
}
