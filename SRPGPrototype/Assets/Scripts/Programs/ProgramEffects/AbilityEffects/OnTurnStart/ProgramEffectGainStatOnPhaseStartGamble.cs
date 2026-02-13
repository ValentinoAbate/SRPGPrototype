using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectGainStatOnPhaseStartGamble : ProgramEffectGainStatOnPhaseStart
{
    public Stats.StatName failureStat;
    public UnitNumber failureNumber;
    [Range(0, 1)]
    [SerializeField] private float successChance = 0.5f;

    public override void Ability(BattleGrid grid, Unit unit)
    {
        if (RandomUtils.RandomU.instance.RollSuccess(successChance))
        {
            base.Ability(grid, unit);
            unit.OnGambleFn?.Invoke(grid, null, unit, true);
        }
        else
        {
            unit.ModifyStat(grid, failureStat, failureNumber.Value(unit), unit);
            unit.OnGambleFn?.Invoke(grid, null, unit, false);
        }
    }
}
