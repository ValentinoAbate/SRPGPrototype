using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectIncomingDamageModActionEffectFiltered : ProgramEffectAddIncomingDamageModifierAbility
{
    [SerializeField] private Action.Type[] actionTypeFilter = new Action.Type[0];
    [SerializeField] private SubAction.Type[] subTypeFilter = new SubAction.Type[0];
    [SerializeField] private ActionEffectDamage damageMod;

    public override int Ability(BattleGrid grid, Action action, SubAction sub, Unit self, Unit source, int damage, ActionEffectDamage.TargetStat targetStat, bool simulation)
    {
        if (!ActionFilters.IsAnyTypeAndSubTypeOptional(action, actionTypeFilter, sub, subTypeFilter))
            return 0;
        return damageMod.BaseDamage(grid, action, self, System.Array.Empty<Vector2Int>());
    }
}
