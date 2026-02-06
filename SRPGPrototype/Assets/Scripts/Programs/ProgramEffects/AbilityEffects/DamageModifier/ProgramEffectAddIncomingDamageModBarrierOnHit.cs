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
        Activate(self);
    }

    public void OnPhaseEnd(BattleGrid grid, Unit unit)
    {
        active = false;
        Destroy(barrierObject);
    }

    private void Activate(Unit self)
    {
        active = true;
        barrierObject = Instantiate(barrierPrefab, self.FxContainer);
    }

    public override bool CanSave(bool isBattle) => isBattle;

    public override string Save(bool isBattle)
    {
        if (!isBattle)
            return string.Empty;
        return BoolUtils.ToStringInt(active);
    }

    public override void Load(string data, bool isBattle, Unit unit)
    {
        if (!isBattle || unit == null)
            return;
        if (active = BoolUtils.FromStringInt(data))
        {
            Activate(unit);
        }
    }
}
