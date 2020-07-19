using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddAction : ProgramEffect
{
    public Action action;

    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.actions.Add(action);
    }
}
