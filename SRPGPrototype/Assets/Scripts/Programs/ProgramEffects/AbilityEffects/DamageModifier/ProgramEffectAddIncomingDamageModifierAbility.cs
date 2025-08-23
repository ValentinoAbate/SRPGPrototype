using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectAddIncomingDamageModifierAbility : ProgramEffectAddAbility
{
    [SerializeField] private string abilityName;

    public override string AbilityName => abilityName;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.incomingDamageMods += Ability;
    }

    public abstract int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat, bool simulation);
}
