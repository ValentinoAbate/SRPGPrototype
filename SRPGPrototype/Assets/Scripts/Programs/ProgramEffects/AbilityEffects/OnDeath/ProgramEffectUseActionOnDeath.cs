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
    public bool triggerOnSelfDestruct = true;

    protected override string AbilityName => "Use " + action.DisplayName + "when destroyed";

    public override void Ability(BattleGrid grid, Unit self, Unit killedBy)
    {
        if (!triggerOnSelfDestruct && killedBy == self)
            return;
        var targetPos = target == Target.Self ? self.Pos : killedBy.Pos;
        action.UseAll(grid, self, targetPos, false);
    }
}
