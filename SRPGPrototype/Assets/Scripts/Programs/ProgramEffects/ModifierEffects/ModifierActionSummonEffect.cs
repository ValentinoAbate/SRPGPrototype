using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierActionSummonEffect : ModifierAction
{
    public override bool AppliesTo(SubAction sub)
    {
        return base.AppliesTo(sub) && sub.HasSubType(SubAction.Type.Summon);
    }

    public abstract void OnSummonEffect(BattleGrid grid, Unit summoner, Unit summoned);
}
