using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramTriggerConditionGamblesStreak : PorgramTriggerConditionOnGamble
{
    public override string RevealedConditionText => $"{(success ? "Succeed at" : "Fail")} {number} gambles in a row ({(Completed ? "Done" : progress + "/" + number)})";

    public override bool Completed => progress >= number;

    [SerializeField] private bool success;
    [SerializeField] private int number;

    private int progress;

    protected override void OnGamble(BattleGrid grid, Action action, Unit unit, bool gambleSuccess)
    {
        if(gambleSuccess == success)
        {
            ++progress;
        }
        else
        {
            progress = 0;
        }
    }

    public override string Save()
    {
        return progress.ToString();
    }

    public override void Load(string data)
    {
        if(int.TryParse(data, out int savedProgress))
        {
            progress = savedProgress;
        }
    }
}
