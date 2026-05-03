using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviorReactOnActionUsed : UnitBehavior
{
    [SerializeField] private AIUnit self;
    [SerializeField] private Unit.Team[] targetTeams;

    void Start()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnAfterAction -= OnAfterAction;
            EncounterEventManager.main.OnAfterAction += OnAfterAction;
        }
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
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
            Cleanup();
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
