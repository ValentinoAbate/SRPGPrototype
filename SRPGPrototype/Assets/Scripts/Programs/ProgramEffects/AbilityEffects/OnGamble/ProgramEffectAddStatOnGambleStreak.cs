using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnGambleStreak : ProgramEffectAddOnGambleAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private int quantity;
    [SerializeField] private bool success;
    [SerializeField] private int streakRequired;

    private int streak = 0;
    public override string AbilityName => $"Gain {quantity} {stat} after a {streakRequired} Gamble {(success ? "Success" : "Failure")} streak ({streak}/{streakRequired})";

    protected override void OnGamble(BattleGrid grid, Action action, Unit unit, bool succeeded)
    {
        if (succeeded == success)
        {
            if (++streak >= streakRequired)
            {
                streak = 0;
                unit.ModifyStat(grid, stat, quantity, unit);
            }
        }
        else
        {
            streak = 0;
        }
    }

    public override bool CanSave(bool isBattle) => true;

    public override string Save(bool isBattle)
    {
        return streak.ToString();
    }

    public override void Load(string data, bool isBattle, Unit unit)
    {
        if(int.TryParse(data, out int savedStreak))
        {
            streak = savedStreak;
        }
    }
}
