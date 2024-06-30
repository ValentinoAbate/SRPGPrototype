using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();
    private readonly List<PlayerUnit> units = new List<PlayerUnit>();

    public override IEnumerator OnPhaseStart()
    {
        if (CheckEndBattle())
            yield break;
        units.Clear();
        units.AddRange(GetUnits<PlayerUnit>());
        foreach (var unit in units)
        {
            if (!unit.Dead)
            {
                yield return StartCoroutine(unit.OnPhaseStart(Grid));
            }
        }
        BattleUI.BeginPlayerTurn();
    }

    public override IEnumerator OnPhaseEnd()
    {
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
        {
            if (!unit.Dead)
            {
                yield return StartCoroutine(unit.OnPhaseEnd());
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
        return true;
    }
}
