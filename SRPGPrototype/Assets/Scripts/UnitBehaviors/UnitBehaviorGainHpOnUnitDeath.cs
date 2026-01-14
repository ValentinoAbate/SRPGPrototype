using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviorGainHpOnUnitDeath : UnitBehavior
{
    [SerializeField] private AIUnit self;
    [SerializeField] private List<Unit.Team> teams;
    [SerializeField] private int amount = 2;
    void Start()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnUnitDeath -= OnUnitDeath;
            EncounterEventManager.main.OnUnitDeath += OnUnitDeath;
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
