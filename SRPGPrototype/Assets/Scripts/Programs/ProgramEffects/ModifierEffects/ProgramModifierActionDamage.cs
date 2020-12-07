public class ProgramModifierActionDamage : ProgramModifierAction
{
    public ActionEffectDamage[] DamageModifiers { get; private set; }

    private void Awake()
    {
        DamageModifiers = GetComponents<ActionEffectDamage>();
    }

    public override bool AppliesTo(SubAction sub)
    {
        return sub.DealsDamage;
    }
}
