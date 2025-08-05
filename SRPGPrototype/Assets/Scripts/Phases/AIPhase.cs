using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPhase : Phase
{
    public const float nextTurnDelay = 0.025f;
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();

    [SerializeField] private Unit.Team team = Unit.Team.None;
    [SerializeField] private bool canEndBattle = false;

    public override IEnumerator OnPhaseStart()
    {
        if (CheckEndBattle())
            yield break;
        var units = new List<AIUnit>(GetUnits<AIUnit>());
        units.Sort();
        foreach (var unit in units)
        {
            if (unit.Dead)
            {
                continue;
            }
            var phaseStartCr = unit.OnPhaseStart(Grid);
            if (phaseStartCr != null)
            {
                yield return phaseStartCr;
            }
        }
        bool firstUnitGone = false;
        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];
            // is unit AP is 0 or it has no AI, skip
            if (unit.Dead || unit.AP <= 0 || unit.AI == null)
                continue;
            if(firstUnitGone)
            {
                yield return new WaitForSeconds(nextTurnDelay);
            }
            firstUnitGone = true;
            var turnCr = unit.DoTurn(Grid);
            if(turnCr != null)
            {
                yield return turnCr;
            }
            if (CheckEndBattle())
                yield break;
        }
        EndPhase();
    }

    protected override bool UnitPredicate(Unit unit)
    {
        return unit.UnitTeam == team;
    }

    public override IEnumerator OnPhaseEnd()
    {
        if (CheckEndBattle())
            yield break;
        var units = new List<AIUnit>(GetUnits<AIUnit>());
        units.Sort();
        foreach (var unit in units)
        {
            if (unit.Dead)
            {
                continue;
            }
            var phaseEndCr = unit.OnPhaseEnd(Grid);
            if(phaseEndCr != null)
            {
                yield return phaseEndCr;
            }
        }
    }

    public bool CheckEndBattle()
    {
        if (!canEndBattle)
            return false;
        if (Grid.MainPlayerDead)
        {
            EndBattle();
            return true;
        }
        return false;
    }
}
