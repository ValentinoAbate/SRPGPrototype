using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddAction : ProgramEffect
{
    public Action action;

    public override void ApplyEffect(Program program, ref Stats stats, ref List<Action> actions)
    {
        actions.Add(action);
    }
}
