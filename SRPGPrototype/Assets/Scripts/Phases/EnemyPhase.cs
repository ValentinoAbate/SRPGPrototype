using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();
    public BattleGrid grid;

    public List<UnitEnemy> units = new List<UnitEnemy>();

    public override IEnumerator OnPhaseStart()
    {
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
            yield return StartCoroutine(unit.OnPhaseStart());
        units.RemoveAll((u) => u.AP <= 0);
        foreach (var unit in units)
        {
            yield return StartCoroutine(unit.DoTurn(grid));
            if (CheckEndPhase())
                yield break;
        }
        EndPhase();
    }

    public override IEnumerator OnPhaseEnd()
    {
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
            yield return StartCoroutine(unit.OnPhaseEnd());
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
