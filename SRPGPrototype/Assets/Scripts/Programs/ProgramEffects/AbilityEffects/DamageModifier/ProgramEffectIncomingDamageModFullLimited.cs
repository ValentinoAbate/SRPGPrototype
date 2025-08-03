using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectIncomingDamageModFullLimited : ProgramEffectAddIncomingDamageModifierAbility
{
    [SerializeField] private Action.Type[] actionTypeFilter = new Action.Type[0];
    [SerializeField] private SubAction.Type[] subTypeFilter = new SubAction.Type[0];
    [SerializeField] private UseLimiter limiter;

    protected override string AbilityName => $"{base.AbilityName} {limiter.DisplayName}";

    public override void ApplyEffect(ref Shell.CompileData data)
    {
        base.ApplyEffect(ref data);
        limiter.Attach(data);
    }

    public override int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat)
    {
        if (damage <= 0 || !ActionFilters.IsAnyTypeAndSubTypeOptional(action, actionTypeFilter, sub, subTypeFilter) || !limiter.TryUse())
            return 0;
        return -damage;
    }
}
