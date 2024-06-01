using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPhase : Phase
{
    public const float nextTurnDelay = 0.025f;
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle();
    public BattleGrid grid;

    [SerializeField] private Unit.Team team = Unit.Team.None;
    [SerializeField] private bool canEndBattle = false;

    private readonly List<AIUnit> units = new List<AIUnit>();

    public override IEnumerator OnPhaseStart(IEnumerable<Unit> allUnits)
    {
        units.Clear();
        units.AddRange(allUnits.Where((u) => u is AIUnit && UnitPredicate(u as AIUnit)).Select((u) => u as AIUnit));
        RemoveAllDead();
        if (CheckEndBattle())
            yield break;
        foreach (var unit in units)
            yield return StartCoroutine(unit.OnPhaseStart());
        foreach (var unit in units)
        {
            // Skip units that are dead or have been reduced to 0 AP or have no AI
            if (unit == null || unit.Dead || unit.AP <= 0 || unit.AI == null)
                continue;
            yield return StartCoroutine(unit.DoTurn(grid));
            if (CheckEndPhase())
                yield break;
            yield return new WaitForSeconds(nextTurnDelay);
        }
        EndPhase();
    }

    private bool UnitPredicate(AIUnit unit)
    {
        return unit.UnitTeam == team;
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
        if (units.Any((u) => u != null && !u.Dead && u.AP > 0))
            return false;
        EndPhase();
        return true;
    }

    public bool CheckEndBattle()
    {
        if (!canEndBattle)
            return false;
        if (units.Count <= 0 || grid.MainPlayerDead)
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
