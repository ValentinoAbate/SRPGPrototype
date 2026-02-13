using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramTriggerConditionGambles : PorgramTriggerConditionOnGamble
{
    public override string RevealedConditionText => $"{(success ? "Succeed at" : "Fail")} {number} gambles ({(Completed ? "Done" : progress + "/" + number)})";

    public override bool Completed => progress >= number;

    [SerializeField] private bool success;
    [SerializeField] private int number;

    protected int progress;

    protected override void OnGamble(BattleGrid grid, Action action, Unit unit, bool gambleSuccess)
    {
        if (gambleSuccess == success)
        {
            ++progress;
        }
    }

    public override string Save()
    {
        return progress.ToString();
    }

    public override void Load(string data)
    {
        if (int.TryParse(data, out int savedProgress))
        {
            progress = savedProgress;
        }
    }
}
