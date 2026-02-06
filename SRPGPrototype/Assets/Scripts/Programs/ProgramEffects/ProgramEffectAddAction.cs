using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddAction : ProgramEffect
{
    public Action action;
    public bool isSecondary;

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

    public override bool CanSave(bool isBattle) => true;

    public override string Save(bool isBattle)
    {
        return action.Save(isBattle);
    }

    public override void Load(string data, bool isBattle, Unit unit)
    {
        action.Load(data, isBattle);
    }
}
