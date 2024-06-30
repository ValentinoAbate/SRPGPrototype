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

    private readonly List<AIUnit> units = new List<AIUnit>();

    public override IEnumerator OnPhaseStart()
    {
        if (CheckEndBattle())
            yield break;
        units.Clear();
        units.AddRange(GetUnits<AIUnit>());
        units.Sort();
        foreach (var unit in units)
        {
            if (unit.Dead)
            {
                continue;
            }
            yield return StartCoroutine(unit.OnPhaseStart(Grid));
        }
        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];
            // is unit AP is 0 or it has no AI, skip
            if (unit.Dead || unit.AP <= 0 || unit.AI == null)
                continue;
            yield return StartCoroutine(unit.DoTurn(Grid)); 
            if (CheckEndBattle())
                yield break;
            if (CheckEndPhase(units, i + 1))
                yield break;
            yield return new WaitForSeconds(nextTurnDelay);
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
        foreach (var unit in units)
        {
            if (unit.Dead)
            {
                continue;
            }
            yield return StartCoroutine(unit.OnPhaseEnd());
        }

    }

    public bool CheckEndPhase(IReadOnlyList<AIUnit> units, int startIndex)
    {
        for(int i = startIndex; i < units.Count; ++i)
        {
            var unit = units[i];
            if (!unit.Dead && unit.AP > 0)
                return false;
        }
        EndPhase();
        return true;
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
