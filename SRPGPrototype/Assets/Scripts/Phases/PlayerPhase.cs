using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();

    public BattleUI ui;

    private List<PlayerUnit> units = new List<PlayerUnit>();

    public override void InitializePhase(IEnumerable<Unit> allUnits)
    {
        units = new List<PlayerUnit>(allUnits.Where((u) => u is PlayerUnit).Select((u) => u as PlayerUnit));
        units.ForEach((u) => u.Actions.ForEach((a) => a.ResetUses(Action.Trigger.EncounterStart)));
    }


    public override IEnumerator OnPhaseStart(IEnumerable<Unit> allUnits)
    {
        units.Clear();
        units.AddRange(allUnits.Where((u) => u is PlayerUnit).Select((u) => u as PlayerUnit));
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
            yield return StartCoroutine(unit.OnPhaseStart());
        ui.BeginPlayerTurn();
    }

    public override IEnumerator OnPhaseEnd()
    {
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
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
