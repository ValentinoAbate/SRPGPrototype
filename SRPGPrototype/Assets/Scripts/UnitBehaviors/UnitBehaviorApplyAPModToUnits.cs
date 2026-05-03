using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviorApplyAPModToUnits : UnitBehavior
{
    [SerializeField] private Unit.PassiveEffect effectKey;
    [SerializeField] private int modifier;
    [SerializeField] private List<Unit.Team> applyToTeams;

    protected override void Awake()
    {
        base.Awake();
        self.OnSpawned += OnSpawned;
        self.OnDeathFn += RemoveEffect;
        self.OnRemoved += RemoveEffect;
    }

    protected override void AttachListeners()
    {
        if (EncounterEventManager.Ready)
        {
            EncounterEventManager.main.OnUnitSpawned += TryApplyEffectToUnit;
            EncounterEventManager.main.OnUnitRemoved += TryRemoveEffectFromUnit;
        }
    }

    protected override void CleanupListeners()
    {
        EncounterEventManager.main.OnUnitSpawned -= TryApplyEffectToUnit;
        EncounterEventManager.main.OnUnitRemoved -= TryRemoveEffectFromUnit;
    }

    private void OnSpawned(BattleGrid grid, Unit self)
    {
        foreach (var unit in grid)
        {
            TryApplyEffectToUnit(unit);
        }
    }

    private void RemoveEffect(BattleGrid grid, Unit self, Unit killedBy)
    {
        RemoveEffect(grid, self);
    }

    private void RemoveEffect(BattleGrid grid, Unit self)
    {
        CleanupListeners();
        foreach (var unit in grid)
        {
            TryRemoveEffectFromUnit(unit);
        }
    }

    private void TryRemoveEffectFromUnit(Unit unit)
    {
        if (applyToTeams.Contains(unit.UnitTeam) && unit.RemovePassiveEffect(effectKey))
        {
            unit.AP -= modifier;
            unit.MaxAP -= modifier;
        }
    }

    private void TryApplyEffectToUnit(Unit unit)
    {
        if (applyToTeams.Contains(unit.UnitTeam) && unit.AddPassiveEffect(effectKey))
        {
            unit.AP += modifier;
            unit.MaxAP += modifier;
        }
    }
}
