using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddAction : ProgramEffect
{
    public Action action;

    public override void Initialize(Program program)
    {
        base.Initialize(program);
        action = action.Validate(transform);
        action.Program = program;
    }

    public override void ApplyEffect(ref Shell.CompileData data)
    {
        data.actions.Add(action);
    }
}
