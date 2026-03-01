using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectDamageModSameColorAdj : ActionEffectDamage
{
    [SerializeField] private int dmgModifier;
    [SerializeField] private Modifier mod;
    [SerializeField] private Program.Color color;
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return BasicDamage(action, user);
    }

    public override int BasicDamage(Action action, Unit user)
    {
        if (mod.Program == null || mod.Program.Shell == null)
            return 0;
        int count = 0;
        foreach (var prog in ProgramModifier.AdjacentPrograms(mod.Program.Shell, mod.Program))
        {
            if(color == prog.color)
            {
                ++count;
            }
        }
        return count + dmgModifier;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
