using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();

    public BattleUI ui;

    public List<UnitPlayer> units = new List<UnitPlayer>();

    public override IEnumerator OnPhaseStart()
    {
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
            yield return StartCoroutine(unit.OnPhaseStart());
        ui.PlayerPhaseUIEnabled = true;
    }

    public override IEnumerator OnPhaseEnd()
    {
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
            yield return StartCoroutine(unit.OnPhaseEnd());
        ui.PlayerPhaseUIEnabled = false;
    }

    public bool CheckEndPhase()
    {
        RemoveAllDead();
        if (units.Any((u) => u.AP > 0))
            return false;
        EndPhase();
        return true;
    }

    public bool CheckEndBattle()
    {
        if (units.Count <= 0)
        {
            EndBattle();
            return true;
        }
        return false;
    }

    public void RemoveAllDead()
    {
        units.RemoveAll((u) => u == null || u.Dead);
    }
}
