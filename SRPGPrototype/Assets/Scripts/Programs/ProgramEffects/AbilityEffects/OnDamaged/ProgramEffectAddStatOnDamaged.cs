using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnDamaged : ProgramEffectAddOnDamagedAbility
{
    public Stats.StatName stat;
    public UnitNumber number;
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;

    public override void Ability(BattleGrid grid, Unit self, Unit source, int amount)
    {
        int value = number.Value(self);
        void ApplyStat()
        {
            self.ModifyStat(grid, stat, value, self);
        }
        EncounterEventManager.EnqueueDelayedEffect(ApplyStat);
    }
}
