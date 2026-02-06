using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectIncomingDamageModFullLimited : ProgramEffectAddIncomingDamageModifierAbility
{
    [SerializeField] private Action.Type[] actionTypeFilter = new Action.Type[0];
    [SerializeField] private SubAction.Type[] subTypeFilter = new SubAction.Type[0];
    [SerializeField] private UseLimiter limiter;

    public override string AbilityName => $"{base.AbilityName} {limiter.DisplayName}";

    public override void ApplyEffect(ref Shell.CompileData data)
    {
        base.ApplyEffect(ref data);
        limiter.Attach(data);
    }

    public override int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat, bool simulation)
    {
        if (damage <= 0 || !ActionFilters.IsAnyTypeAndSubTypeOptional(action, actionTypeFilter, sub, subTypeFilter) || !limiter.CanUse)
            return 0;
        if (!simulation)
        {
            limiter.TryUse();
        }
        return -damage;
    }

    public override bool CanSave(bool isBattle) => isBattle;

    public override string Save(bool isBattle)
    {
        if (!isBattle)
            return string.Empty;
        return limiter.Save();
    }

    public override void Load(string data, bool isBattle, Unit unit)
    {
        if (isBattle)
        {
            limiter.Load(data);
        }
    }
}
