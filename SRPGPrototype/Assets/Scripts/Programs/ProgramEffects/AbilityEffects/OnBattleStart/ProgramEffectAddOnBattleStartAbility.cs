public abstract class ProgramEffectAddOnBattleStartAbility : ProgramEffectAddAbility
{
    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.abilityOnBattleStart += Ability;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
}
