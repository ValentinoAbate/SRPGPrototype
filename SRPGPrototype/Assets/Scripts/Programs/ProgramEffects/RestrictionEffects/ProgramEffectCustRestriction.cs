public abstract class ProgramEffectCustRestriction : ProgramEffect
{
    public const string noErrorMessage = "No Compile Error";
    public override void ApplyEffect(ref Shell.CompileData data)
    {
        data.restrictions.Add(Restriction);
        data.restrictionNames.Add(RestrictionName);
    }

    protected abstract bool Restriction(Shell shell, out string errorMessage);

    protected abstract string RestrictionName { get; }
}
