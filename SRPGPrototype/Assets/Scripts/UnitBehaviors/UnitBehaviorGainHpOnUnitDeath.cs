using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviorGainHpOnUnitDeath : UnitBehavior
{
    [SerializeField] private List<Unit.Team> teams;
    [SerializeField] private int amount = 2;

    protected override void AttachListeners()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnUnitDeath -= OnUnitDeath;
            EncounterEventManager.main.OnUnitDeath += OnUnitDeath;
        }
    }

    protected override void CleanupListeners()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnUnitDeath -= OnUnitDeath;
        }
    }

    private void OnUnitDeath(BattleGrid grid, Unit unit, Unit killedBy)
    {
        if (!teams.Contains(unit.UnitTeam) || unit == self)
            return;
        self.MaxHP += amount;
        self.Heal(amount, self);
    }
}
