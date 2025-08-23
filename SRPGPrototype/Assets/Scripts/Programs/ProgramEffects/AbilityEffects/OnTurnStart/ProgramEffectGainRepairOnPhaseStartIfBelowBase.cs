using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectGainRepairOnPhaseStartIfBelowBase : ProgramEffectAddOnPhaseStartAbility
{
    public override string AbilityName => "Gain 1 Repair on turn start if below Base";

    public override void Ability(BattleGrid grid, Unit unit)
    {
        if (unit.Repair >= unit.BaseRepair)
            return;
        unit.Repair++;
    }
}
