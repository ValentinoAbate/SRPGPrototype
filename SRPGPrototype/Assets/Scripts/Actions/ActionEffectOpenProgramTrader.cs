using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectOpenProgramTrader : ActionEffect
{
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        UIManager.main.ProgramTraderUI.Show(user);
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        return true;
    }
}
