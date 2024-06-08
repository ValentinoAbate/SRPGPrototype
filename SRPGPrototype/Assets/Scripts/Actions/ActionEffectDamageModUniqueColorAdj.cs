using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectDamageModUniqueColorAdj : ActionEffectDamage
{
    [SerializeField] private int dmgModifier;
    [SerializeField] private Modifier mod;
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return BasicDamage(action, user);
    }

    public override int BasicDamage(Action action, Unit user)
    {
        if (mod.Program == null || mod.Program.Shell == null)
            return 0;
        var colors = new HashSet<Program.Color>();
        foreach (var prog in ProgramModifier.AdjacentPrograms(mod.Program.Shell, mod.Program))
        {
            if (colors.Contains(prog.color))
                continue;
            colors.Add(prog.color);
        }
        return colors.Count + dmgModifier;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
