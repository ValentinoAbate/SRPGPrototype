using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddIncomingDamageModBarrierTemporary : ProgramEffectIncomingDamageModBarrier
{
    private const int uninitializedTurns = -1;
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

    public int TurnsLeft { get; private set; } = uninitializedTurns;
    private GameObject barrierObject = null;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        base.AddAbility(ref data);
        data.onPhaseEnd += OnPhaseEnd;
        data.onSpawned += OnSpawned;
    }

    protected override void AddAbility(Unit unit)
    {
        base.AddAbility(unit);
        unit.OnPhaseEndFn += OnPhaseEnd;
        unit.OnSpawned += OnSpawned;
    }

    public override int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat, bool simulation)
    {
        if (TurnsLeft <= 0)
            return 0;
        return base.Ability(grid, action, sub, self, source, damage, targetStat, simulation);
    }

    public void OnSpawned(BattleGrid grid, Unit unit)
    {
        if(numTurns > 0 && TurnsLeft == uninitializedTurns)
        {
            Activate(numTurns, unit);
        }
    }

    private void Activate(int turns, Unit unit)
    {
        TurnsLeft = turns;
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

    public override bool CanSave(bool isBattle) => isBattle;

    public override string Save(bool isBattle)
    {
        if (!isBattle)
            return string.Empty;
        return TurnsLeft.ToString();
    }

    public override void Load(string data, bool isBattle, Unit unit)
    {
        if (!isBattle || unit == null)
            return;
        if(int.TryParse(data, out int savedTurnsLeft))
        {
            TurnsLeft = savedTurnsLeft;
            if(TurnsLeft > 0)
            {
                Activate(TurnsLeft, unit);
            }
        }
    }
}
