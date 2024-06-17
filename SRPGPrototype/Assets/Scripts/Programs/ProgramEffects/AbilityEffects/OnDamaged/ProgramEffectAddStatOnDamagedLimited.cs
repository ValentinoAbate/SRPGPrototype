using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnDamagedLimited : ProgramEffectAddStatOnDamaged
{
    [SerializeField] private UseLimiter limiter;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        base.AddAbility(ref data);
        limiter.Attatch(data);
    }

    protected override void Ability(BattleGrid grid, Unit self, Unit source, int amount)
    {
        if (!limiter.TryUse())
        {
            return;
        }
        base.Ability(grid, self, source, amount);
    }
}
