using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatToOtherOnRepositionOtherTag : ProgramEffectAddStatToOtherOnRepositionOther
{
    [SerializeField] private Unit.Tags requiredTags;
    [SerializeField] private BooleanOperator op;
    public override void Ability(BattleGrid grid, Unit source, Unit target)
    {
        if (op.Evaluate(target.UnitTags, requiredTags))
        {
            base.Ability(grid, source, target);
        }
    }
}
