public abstract class ProgramEffectAddOnPhaseEndAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onPhaseEnd += Ability;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.OnPhaseEndFn += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
}
