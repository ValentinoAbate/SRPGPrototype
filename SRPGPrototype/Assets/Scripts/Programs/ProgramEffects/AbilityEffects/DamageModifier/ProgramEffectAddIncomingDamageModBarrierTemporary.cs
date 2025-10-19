using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddIncomingDamageModBarrierTemporary : ProgramEffectIncomingDamageModBarrier
{
    [SerializeField] private int numTurns;
    [SerializeField] private GameObject barrierPrefab;

    public override string AbilityName => TurnsLeft == 0 ? "Vulnerable" : $"Invulnerable to all damage for {TurnsText}";
    private string TurnsText
    {
        get
        {
            int turnsCount = (TurnsLeft < 0 ? numTurns : TurnsLeft);
            return turnsCount == 1 ? "1 turn" : $"{turnsCount} turns";
        }
    }

    public int TurnsLeft { get; private set; } = -1;
    private GameObject barrierObject = null;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        base.AddAbility(ref data);
        data.onPhaseEnd += OnPhaseEnd;
        data.onBattleStart += OnBattleStart;
    }

    protected override void AddAbility(Unit unit)
    {
        base.AddAbility(unit);
        unit.OnPhaseEndFn += OnPhaseEnd;
        unit.OnBattleStartFn += OnBattleStart;
    }

    public override int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat, bool simulation)
    {
        if (TurnsLeft <= 0)
            return 0;
        return base.Ability(grid, action, sub, self, source, damage, targetStat, simulation);
    }

    public void OnBattleStart(BattleGrid grid, Unit unit)
    {
        TurnsLeft = numTurns;
        barrierObject = Instantiate(barrierPrefab, unit.FxContainer);
    }

    public void OnPhaseEnd(BattleGrid grid, Unit unit)
    {
        if(TurnsLeft > 0)
        {
            if(--TurnsLeft <= 0 && barrierObject != null)
            {
                Destroy(barrierObject);
            }
        }

    }
}
