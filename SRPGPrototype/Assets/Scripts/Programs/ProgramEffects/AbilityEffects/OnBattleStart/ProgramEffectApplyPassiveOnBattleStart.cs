using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectApplyPassiveOnBattleStart : ProgramEffectAddOnBattleStartAbility
{
    [SerializeField] private string abilityName;
    public override string AbilityName => abilityName;

    [SerializeField] private Unit.PassiveEffect effect;

    public override void Ability(BattleGrid grid, Unit unit)
    {
        unit.AddPassiveEffect(effect);
    }
}
