using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectDamageModNumber : ActionEffectDamage
{
    [SerializeField] private Modifier mod;
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, List<Vector2Int> targetPositions)
    {
        if (action.Program == null || action.Program.Shell == null)
            return 0;
        return action.Program.Shell.ModifierMap.Count((kvp) => kvp.Value.Contains(mod));
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
