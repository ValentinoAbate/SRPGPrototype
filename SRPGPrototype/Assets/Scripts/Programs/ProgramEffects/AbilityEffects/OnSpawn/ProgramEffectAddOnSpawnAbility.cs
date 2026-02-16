public abstract class ProgramEffectAddOnSpawnAbility : ProgramEffectAddAbility
{
    protected override void AddAbility(ref Shell.CompileData data)
    {
        data.onSpawned += Ability;
        data.onRemoved += Cleanup;
    }

    protected override void AddAbility(Unit unit)
    {
        unit.OnSpawned += Ability;
        unit.OnRemoved += Cleanup;
    }

    public abstract void Ability(BattleGrid grid, Unit unit);
    public abstract void Cleanup(BattleGrid grid, Unit unit);
}
