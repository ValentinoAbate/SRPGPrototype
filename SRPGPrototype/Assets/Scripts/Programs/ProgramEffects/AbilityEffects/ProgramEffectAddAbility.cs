public abstract class ProgramEffectAddAbility : ProgramEffect
{
    public override void ApplyEffect(ref Shell.CompileData data)
    {
        AddAbility(ref data);
        data.abilityNames.Add(AbilityName);
    }

    public void ApplyEffect(Unit unit)
    {
        AddAbility(unit);
    }

    protected abstract void AddAbility(ref Shell.CompileData data);

    protected abstract void AddAbility(Unit unit);

    public abstract string AbilityName { get; }
}
