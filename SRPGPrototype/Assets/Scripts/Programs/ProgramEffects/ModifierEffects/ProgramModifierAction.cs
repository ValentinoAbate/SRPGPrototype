using System.Linq;

public abstract class ProgramModifierAction : ProgramModifier
{
    public override bool AppliesTo(Program p)
    {
        foreach(var effect in p.Effects)
        {
            var addActionEffect = effect as ProgramEffectAddAction;
            if (addActionEffect == null)
                continue;
            if (addActionEffect.action.subActions.Any(AppliesTo))
                return true;
        }
        return false;
    }

    public abstract bool AppliesTo(SubAction sub);
}
