using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffectCustRestriction : ProgramEffect
{
    public const string noErrorMessage = "No Compile Error";
    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.restrictions.Add(Restriction);
    }

    protected abstract bool Restriction(Shell shell, out string errorMessage);
}
