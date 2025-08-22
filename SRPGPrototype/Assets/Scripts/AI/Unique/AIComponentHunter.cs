using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentHunter : AIComponentBasic
{
    [SerializeField] private ProgramEffectAddIncomingDamageModBarrierTemporary barrierEffect;
    public override void Initialize(AIUnit self)
    {
        base.Initialize(self);
        self.OnPhaseEndFn += OnPhaseEnd;
        self.OnBattleStartFn += barrierEffect.OnBattleStart;
    }

    private void OnPhaseEnd(BattleGrid grid, Unit unit)
    {
        if(barrierEffect.TurnsLeft > 0)
        {
            barrierEffect.OnPhaseEnd(grid, unit);
            if(barrierEffect.TurnsLeft == 0)
            {
                options |= Options.PrioritizeDistance | Options.RunAwayAfterAttacking;
            }
        }
    }
}
