using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectIncomingDamageModBarrier : ProgramEffectAddIncomingDamageModifierAbility
{
    public override int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat, bool simulation)
    {
        return -damage;
    }
}
