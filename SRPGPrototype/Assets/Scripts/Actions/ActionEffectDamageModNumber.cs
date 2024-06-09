using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectDamageModNumber : ActionEffectDamage
{
    [SerializeField] private Modifier mod;
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return BasicDamage(action, user);
    }

    public override int BasicDamage(Action action, Unit user)
    {
        var prog = mod.Program;
        if (prog == null || prog.Shell == null || prog.Shell.ModifierMap == null)
            return 0;
        return prog.Shell.ModifierMap.Count((kvp) => kvp.Value.Contains(mod));
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
