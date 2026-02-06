using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnGambleStreak : ProgramEffectAddSubActionAbility
{
    [SerializeField] private Stats.StatName stat;
    [SerializeField] private int quantity;
    [SerializeField] private bool success;
    [SerializeField] private int streakRequired;

    private int streak = 0;
    public override string AbilityName => $"Gain {quantity} {stat} after a {streakRequired} Gamble {(success ? "Success" : "Failure")} streak ({streak}/{streakRequired})";

    public override void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        foreach(var effect in subAction.Effects)
        {
            if (effect is IGambleActionEffect gambleActionEffect && gambleActionEffect.GambleSuccess.HasValue)
            {
                if(gambleActionEffect.GambleSuccess.Value == success)
                {
                    if (++streak >= streakRequired)
                    {
                        streak = 0;
                        user.ModifyStat(grid, stat, quantity, user);
                    }
                }
                else
                {
                    streak = 0;
                }
            }
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
