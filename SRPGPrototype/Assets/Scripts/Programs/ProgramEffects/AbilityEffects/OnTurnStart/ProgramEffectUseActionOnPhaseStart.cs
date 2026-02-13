using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectUseActionOnPhaseStart : ProgramEffectAddOnPhaseStartAbility
{
    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName = string.Empty;
    [SerializeField] private Action action;

    public override void Ability(BattleGrid grid, Unit unit)
    {
        var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
        actionInstance.UseAll(grid, unit, unit.Pos, false);
        Destroy(actionInstance);
    }
}
