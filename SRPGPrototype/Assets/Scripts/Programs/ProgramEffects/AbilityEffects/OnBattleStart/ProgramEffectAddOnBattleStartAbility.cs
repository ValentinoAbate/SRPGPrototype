public abstract class ProgramEffectAddOnBattleStartAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onBattleStart += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
}
