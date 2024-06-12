using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterActionAbilityTurnLimit : ProgramEffectAddStatAfterActionAbility
{
    [SerializeField] UseLimiter useLimiter;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        base.AddAbility(ref data);
        useLimiter.Attatch(data);
    }

    protected override void Ability(BattleGrid grid, Action action, Unit user)
    {
        if (!AppliesToAction(action, user))
            return;
        if (!useLimiter.TryUse())
            return;
        base.Ability(grid, action, user);
    }
}
