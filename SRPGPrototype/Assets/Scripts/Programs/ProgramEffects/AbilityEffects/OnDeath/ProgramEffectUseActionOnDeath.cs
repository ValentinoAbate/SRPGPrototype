public class ProgramEffectUseActionOnDeath : ProgramEffectAddOnDeathAbility, IDetonatable
{
    public enum Target
    { 
        Self,
        Killer,
    }

    public Target target;
    public Action action;
    public bool triggerOnSelfDestruct = true;

    public override string AbilityName => "Use " + action.DisplayName + "when destroyed";

    public Action DetonateAction => action;

    public override void Ability(BattleGrid grid, Unit self, Unit killedBy)
    {
        if (!triggerOnSelfDestruct && killedBy == self)
            return;
        var targetPos = target == Target.Self ? self.Pos : killedBy.Pos;
        var targetUnit = target == Target.Self ? self : killedBy;
        var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
        actionInstance.UseAll(grid, targetUnit, targetPos, false);
        Destroy(actionInstance);
    }

    public void Detonate(BattleGrid grid, Unit self, Unit detonator)
    {
        Ability(grid, self, detonator);
    }
}
