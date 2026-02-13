using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnGamble : ProgramEffectAddOnGambleAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private int quantity;
    [SerializeField] private bool success;
    public override string AbilityName => $"Add {quantity} {stat} after Gamble {(success ? "Success" : "Fail")}";

    protected override void OnGamble(BattleGrid grid, Action action, Unit unit, bool succeeded)
    {
        if (succeeded == success)
        {
            unit.ModifyStat(grid, stat, quantity, unit);
        }
    }
}
