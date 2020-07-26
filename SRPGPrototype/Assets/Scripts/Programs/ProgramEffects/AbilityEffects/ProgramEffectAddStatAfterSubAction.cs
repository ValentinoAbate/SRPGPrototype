using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterSubAction : ProgramEffectAddOnAfterSubActionAbility
{
    public Stats.StatName stat;
    public ActionNumber number;
    public override void Ability(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Vector2Int> targetPositions)
    {
        int value = number.ActionValue(grid, action, user, targetPositions);
        switch (stat)
        {
            case Stats.StatName.HP:
                if(value > 0)
                    user.Damage(value);
                break;
            case Stats.StatName.MaxHP:
                user.MaxHP += value;
                break;
            case Stats.StatName.AP:
                user.AP += value;
                break;
            case Stats.StatName.MaxAP:
                user.MaxAP += value;
                break;
            case Stats.StatName.Repair:
                user.Repair += value;
                break;
            case Stats.StatName.Power:
                user.Power.Value += value;
                break;
            case Stats.StatName.Speed:
                user.Speed.Value += value;
                break;
            case Stats.StatName.Defense:
                user.Defense.Value += value;
                break;
        }
    }
}
