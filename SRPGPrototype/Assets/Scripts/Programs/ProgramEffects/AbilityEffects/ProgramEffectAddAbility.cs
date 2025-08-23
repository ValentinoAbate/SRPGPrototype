public abstract class ProgramEffectAddAbility : ProgramEffect
{
    public override void ApplyEffect(ref Shell.CompileData data)
    {
        AddAbility(ref data);
        data.abilityNames.Add(AbilityName);
    }

    protected abstract void AddAbility(ref Shell.CompileData data);

    public abstract string AbilityName { get; }
}
