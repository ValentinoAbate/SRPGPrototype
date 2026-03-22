using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectNegateHybridPenalty : ProgramEffectAddAbility
{
    public override string AbilityName => "Hybrid penalty immunity";

    protected override void AddAbility(Shell.CompileData data)
    {
        data.applyHybridFailurePenalty = false;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.CanApplyHybridFailurePenalty = false;
    }
}
