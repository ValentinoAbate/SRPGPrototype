using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentHunter : AIComponentBasic
{
    [SerializeField] private ProgramEffectAddIncomingDamageModBarrierTemporary barrierEffect;
    [SerializeField] private Action summonAction;
    [SerializeField] private int reinforcementAmount = 2;
    private Unit self;
    public override void Initialize(AIUnit self)
    {
        this.self = self;
        base.Initialize(self);
        self.OnPhaseEndFn += OnPhaseEnd;
        self.OnSpawned += barrierEffect.OnSpawned;
        self.IncomingDamageMods += barrierEffect.Ability;
        summonAction = summonAction.Validate(self.ActionTransform);
    }

    private void OnPhaseEnd(BattleGrid grid, Unit unit)
    {
        if(barrierEffect.TurnsLeft > 0)
        {
            barrierEffect.OnPhaseEnd(grid, unit);
            if(barrierEffect.TurnsLeft == 0)
            {
                options |= Options.PrioritizeDistance | Options.RunAwayAfterAttacking;
                SummonReinforcements(grid, unit);
            }
        }
    }

    private void SummonReinforcements(BattleGrid grid, Unit self)
    {
        // Check for targetspace in range
        for (int i = 0; i < reinforcementAmount; i++)
        {
            if (self.Dead)
                break;
            var tPos = ChooseRandomEmptyTargetPosition(grid, self, summonAction);
            if (tPos == BattleGrid.OutOfBounds)
                break;
            summonAction.UseAll(grid, self, tPos, false);
        }
    }

    public override bool CanSave => barrierEffect.CanSave(true);
    public override string Save()
    {
        return barrierEffect.Save(true);
    }
    public override void Load(string data)
    {
        barrierEffect.Load(data, true, self);
    }
}
