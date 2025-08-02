﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();

    public override IEnumerator OnPhaseStart()
    {
        if (CheckEndBattle())
            yield break;
        foreach (var unit in GetUnits<Unit>())
        {
            if (!unit.Dead)
            {
                var phaseStartCr = unit.OnPhaseStart(Grid);
                yield return phaseStartCr;
            }
        }
        BattleUI.BeginPlayerTurn();
    }

    public override IEnumerator OnPhaseEnd()
    {
        if (CheckEndBattle())
            yield break;
        foreach (var unit in GetUnits<Unit>())
        {
            if (!unit.Dead)
            {
                var phaseStartCr = unit.OnPhaseEnd();
                if (phaseStartCr != null)
                {
                    yield return phaseStartCr;
                }
            }
        }
        BattleUI.EndPlayerTurn();
    }

    public bool CheckEndBattle()
    {
        if (Grid.MainPlayerDead)
        {
            EndBattle();
            return true;
        }
        return false;
    }

    protected override bool UnitPredicate(Unit unit)
    {
        return UnitFilters.IsPlayer(unit);
    }
}
