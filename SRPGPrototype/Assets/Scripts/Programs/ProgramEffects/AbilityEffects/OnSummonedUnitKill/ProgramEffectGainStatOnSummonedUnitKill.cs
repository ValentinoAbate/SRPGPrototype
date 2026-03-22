using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectGainStatOnSummonedUnitKill : ProgramEffectAddOnSummonedUnitKillAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private UnitNumber number;
    protected override void Ability(BattleGrid grid, Unit self, Unit killed, Unit killer)
    {
        self.ModifyStat(grid, stat, number.Value(self), killer);
    }
}
