using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();

    public BattleUI ui;
    [SerializeField] private BattleGrid grid;

    public List<PlayerUnit> Units { get; } = new List<PlayerUnit>();

    private static int UnitComparer(PlayerUnit u1, PlayerUnit u2)
    {
        return u1.UnitIndex.CompareTo(u2.UnitIndex);
    }


    public override IEnumerator OnPhaseStart(IEnumerable<Unit> allUnits)
    {
        Units.Clear();
        foreach (var unit in allUnits)
        {
            if (unit is PlayerUnit playerUnit)
            {
                Units.Add(playerUnit);
            }
        }
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in Units)
            yield return StartCoroutine(unit.OnPhaseStart(grid));
        ui.BeginPlayerTurn();
    }

    public override IEnumerator OnPhaseEnd()
    {
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in Units)
            yield return StartCoroutine(unit.OnPhaseEnd());
        ui.EndPlayerTurn();
    }

    public bool CheckEndPhase()
    {
        return false;
        //RemoveAllDead();
        //if (units.Any((u) => u.AP > 0))
        //    return false;
        //EndPhase();
        //return true;
    }

    public bool CheckEndBattle()
    {
        if (!Units.Any((u) => u.IsMain && !u.Dead))
        {
            EndBattle();
            return true;
        }
        return false;
    }

    public void RemoveAllDead()
    {
        Units.RemoveAll((u) => u == null || u.Dead);
    }

    public bool TryGetPlayer(int unitIndex, out PlayerUnit player)
    {
        foreach(var unit in Units)
        {
            if(unit.UnitIndex == unitIndex)
            {
                player = unit;
                return true;
            }
        }
        player = null;
        return false;
    }
}
