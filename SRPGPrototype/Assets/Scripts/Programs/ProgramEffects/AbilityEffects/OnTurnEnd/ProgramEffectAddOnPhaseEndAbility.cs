public abstract class ProgramEffectAddOnPhaseEndAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onPhaseEnd += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
}
