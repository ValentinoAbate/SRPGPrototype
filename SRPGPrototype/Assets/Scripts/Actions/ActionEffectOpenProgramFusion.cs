using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectOpenProgramFusion : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        UIManager.main.ProgramFusionUI.Show(user, 100);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return true;
    }
}
