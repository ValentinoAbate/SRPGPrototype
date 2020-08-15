using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ProgramEffectUseActionOnDeath : ProgramEffectAddOnDeathAbility
{
    public enum Target
    { 
        Self,
        Killer,
    }

    public Target target;
    public Action action;

    public override void Ability(BattleGrid grid, Unit self, Unit killedBy)
    {
        var targetPos = target == Target.Self ? self.Pos : killedBy.Pos;
        action.UseAll(grid, self, targetPos, false);
    }
}
