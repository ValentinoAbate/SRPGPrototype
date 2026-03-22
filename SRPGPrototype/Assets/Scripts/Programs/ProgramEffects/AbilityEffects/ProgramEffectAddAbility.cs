public abstract class ProgramEffectAddAbility : ProgramEffect
{
    public override void ApplyEffect(Shell.CompileData data)
    {
        AddAbility(data);
        data.abilityNames.Add(AbilityName);
    }

    public void ApplyEffect(Unit unit)
    {
        AddAbility(unit);
    }

    protected abstract void AddAbility(Shell.CompileData data);

    protected abstract void AddAbility(Unit unit);

    public abstract string AbilityName { get; }
}
