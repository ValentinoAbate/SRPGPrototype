public abstract class ProgramEffectAddOnPhaseStartAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onPhaseStart += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
}
