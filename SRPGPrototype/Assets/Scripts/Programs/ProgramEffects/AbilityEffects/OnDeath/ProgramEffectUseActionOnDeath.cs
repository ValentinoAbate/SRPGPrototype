public class ProgramEffectUseActionOnDeath : ProgramEffectAddOnDeathAbility
{
    public enum Target
    { 
        Self,
        Killer,
    }

    public Target target;
    public Action action;
    public bool triggerOnSelfDestruct = true;

    protected override string AbilityName => "Use " + action.DisplayName + "when destroyed";

    public override void Ability(BattleGrid grid, Unit self, Unit killedBy)
    {
        if (!triggerOnSelfDestruct && killedBy == self)
            return;
        var targetPos = target == Target.Self ? self.Pos : killedBy.Pos;
        var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
        actionInstance.UseAll(grid, self, targetPos, false);
        Destroy(actionInstance);
    }
}
