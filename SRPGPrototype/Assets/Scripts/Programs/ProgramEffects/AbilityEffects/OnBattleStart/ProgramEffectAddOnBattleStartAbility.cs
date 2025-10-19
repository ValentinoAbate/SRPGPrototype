public abstract class ProgramEffectAddOnBattleStartAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onBattleStart += Ability;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.OnBattleStartFn += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
}
