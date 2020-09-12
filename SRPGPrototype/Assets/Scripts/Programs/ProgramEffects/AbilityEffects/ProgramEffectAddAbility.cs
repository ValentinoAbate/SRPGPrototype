public abstract class ProgramEffectAddAbility : ProgramEffect
{
    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        AddAbility(program, ref data);
        data.abilityNames.Add(AbilityName);
    }

    protected abstract void AddAbility(Program program, ref Shell.CompileData data);

    protected abstract string AbilityName { get; }
}
