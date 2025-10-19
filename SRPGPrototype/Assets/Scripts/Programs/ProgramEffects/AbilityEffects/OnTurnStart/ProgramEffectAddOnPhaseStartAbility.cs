public abstract class ProgramEffectAddOnPhaseStartAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onPhaseStart += Ability;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.OnPhaseStartFn += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
}
