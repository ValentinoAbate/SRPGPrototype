using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectUseActionOnKill : ProgramEffectAddOnKillAbility
{
    public enum Target
    {
        Self,
        Killed,
    }
    [SerializeField] private Unit.Team[] teams = new Unit.Team[1] { Unit.Team.Enemy };
    [SerializeField] private Target target;
    [SerializeField] private Target actionUser;
    [SerializeField] private Action action;

    public override string AbilityName => abilityName;
    [SerializeField] private string abilityName;

    protected override void Ability(BattleGrid grid, Unit killed, Unit self)
    {
        if (!UnitPredicate(grid, killed, self))
            return;
        var targetPos = target == Target.Self ? self.Pos : killed.Pos;
        var targetUnit = target == Target.Self ? self : killed;
        var user = actionUser == Target.Self ? self : killed;
        var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
        void ApplyAction()
        {
            actionInstance.UseAll(grid, user, targetPos, false);
            Destroy(actionInstance);
        }
        EncounterEventManager.EnqueueDelayedEffect(ApplyAction);
    }

    protected virtual bool UnitPredicate(BattleGrid grid, Unit killed, Unit self)
    {
        return UnitFilters.IsOnTeam(killed, teams);
    }
}
