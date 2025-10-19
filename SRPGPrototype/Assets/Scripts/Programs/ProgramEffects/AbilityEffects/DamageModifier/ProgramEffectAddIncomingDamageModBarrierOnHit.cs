using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddIncomingDamageModBarrierOnHit : ProgramEffectIncomingDamageModBarrier
{
    [SerializeField] private GameObject barrierPrefab;

    private bool active = false;

    public override string AbilityName => active ? $"Invulnerable to all damage until the end of the turn" : string.Empty;

    private GameObject barrierObject = null;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        base.AddAbility(ref data);
        data.onPhaseEnd += OnPhaseEnd;
        data.onDamaged += OnDamaged;
    }

    protected override void AddAbility(Unit unit)
    {
        base.AddAbility(unit);
        unit.OnPhaseEndFn += OnPhaseEnd;
        unit.OnDamagedFn += OnDamaged;
    }

    public override int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat, bool simulation)
    {
        if (!active)
            return 0;
        return base.Ability(grid, action, sub, self, source, damage, targetStat, simulation);
    }

    private void OnDamaged(BattleGrid grid, Unit self, Unit source, int amount)
    {
        active = true;
        barrierObject = Instantiate(barrierPrefab, self.FxContainer);
    }

    public void OnPhaseEnd(BattleGrid grid, Unit unit)
    {
        active = false;
        Destroy(barrierObject);
    }
}
