using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddAction : ProgramEffect
{
    public Action action;

    private void Awake()
    {
        action = action.Validate(transform);
    }

    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.actions.Add(action);
    }
}
