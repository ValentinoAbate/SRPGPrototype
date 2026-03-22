using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddOnSummonedUnitKillAbility : ProgramEffectAddAbility
{
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName;
    [SerializeField] private Unit.Team[] targetTeams;
    private Unit.OnDeath checkFunction;

    protected override void AddAbility(Shell.CompileData data)
    {
        bool IsSelf(Unit u)
        {
            return u is PlayerUnit && u.Shell == data.shell;
        }
        checkFunction = (BattleGrid grid, Unit unit, Unit killedBy) =>
        {
            CheckForKill(grid, IsSelf, unit, killedBy);
        };
        data.onSpawned += AttachListeners;
        data.onRemoved += RemoveListeners;
        data.onDeath += RemoveListeners;
    }

    protected override void AddAbility(Unit unit)
    {
        bool IsSelf(Unit u)
        {
            return u == unit;
        }
        checkFunction = (BattleGrid grid, Unit unit, Unit killedBy) =>
        {
            CheckForKill(grid, IsSelf, unit, killedBy);
        };
        unit.OnSpawned += AttachListeners;
        unit.OnRemoved += RemoveListeners;
        unit.OnDeathFn += RemoveListeners;
    }

    private void AttachListeners(BattleGrid arg1, Unit arg2)
    {
        if (!EncounterEventManager.Ready)
            return;
        EncounterEventManager.main.OnUnitDeath += checkFunction;
    }

    private void RemoveListeners(BattleGrid grid, Unit unit, Unit killedBy)
    {
        RemoveListeners(grid, unit);
    }

    private void RemoveListeners(BattleGrid grid, Unit unit)
    {
        if (!EncounterEventManager.Ready)
            return;
        EncounterEventManager.main.OnUnitDeath -= checkFunction;
    }

    private void CheckForKill(BattleGrid grid, Predicate<Unit> selfPred, Unit unit, Unit killedBy)
    {
        if (!UnitFilters.IsOnTeam(unit, targetTeams) || selfPred(killedBy))
            return;
        var unitToCheck = killedBy;
        while(unitToCheck != null)
        {
            unitToCheck = unitToCheck.Summoner;
            if (selfPred(unitToCheck))
            {
                Ability(grid, unitToCheck, unit, killedBy);
                return;
            }
        }
    }

    protected abstract void Ability(BattleGrid grid, Unit self, Unit killed, Unit killer);
}
