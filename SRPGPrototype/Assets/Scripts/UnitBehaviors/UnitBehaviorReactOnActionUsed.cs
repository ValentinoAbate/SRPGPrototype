using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviorReactOnActionUsed : UnitBehavior
{
    [SerializeField] private Unit.Team[] targetTeams;

    protected override void AttachListeners()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnAfterAction -= OnAfterAction;
            EncounterEventManager.main.OnAfterAction += OnAfterAction;
        }
    }

    protected override void CleanupListeners()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnAfterAction -= OnAfterAction;
        }
    }

    private void OnAfterAction(BattleGrid grid, Action action, Unit user, int cost)
    {
        if (self.Dead)
        {
            CleanupListeners();
            return;
        }
        if (user == self || !UnitFilters.IsOnTeam(user, targetTeams))
            return;
        Coroutine Reaction()
        {
            return self.StartCoroutine(DoTurn(grid));
        }
        EncounterEventManager.EnqueueReaction(Reaction);
    }

    private IEnumerator DoTurn(BattleGrid grid)
    {
        yield return self.OnPhaseStart(grid);
        yield return self.DoTurn(grid);
        yield return self.OnPhaseEnd(grid);
    }
}
