using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectUseActionOnDamaged : ProgramEffectAddOnDamagedAbility
{
    public enum Target
    {
        Self,
        DamageSource,
        EmptyAdjacentToSelf,
    }

    public Target target;
    public Action action;
    protected override string AbilityName => "Use " + action.DisplayName + "when damaged";

    public override void Ability(BattleGrid grid, Unit self, Unit source, int amount)
    {
        Vector2Int targetPos;
        if(target == Target.Self)
        {
            targetPos = self.Pos;
        }
        else if(target == Target.DamageSource)
        {
            targetPos = source.Pos;
        }
        else if(target == Target.EmptyAdjacentToSelf)
        {
            var validPositions = new List<Vector2Int>(8);
            foreach(var pos in self.Pos.AdjacentBoth())
            {
                if (grid.IsLegalAndEmpty(pos))
                    validPositions.Add(pos);
            }
            if (validPositions.Count == 0)
                return;
            targetPos = RandomUtils.RandomU.instance.Choice(validPositions);
        }
        else
        {
            return;
        }
        var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
        actionInstance.UseAll(grid, self, targetPos, false);
        Destroy(actionInstance);
    }
}
