using System.Linq;
using UnityEngine;

public abstract class ModifierAction : Modifier
{
    [SerializeField] private bool filterByActionType = true;
    [SerializeField] private Action.Type[] actionTypes = new Action.Type[] { Action.Type.Weapon };
    [SerializeField] private bool filterBySubType = false;
    [SerializeField] private SubAction.Type[] subTypes = new SubAction.Type[0];
    protected override bool ModFilter(Program p)
    {
        foreach(var effect in p.Effects)
        {
            var addActionEffect = effect as ProgramEffectAddAction;
            if (addActionEffect == null)
                continue;
            if (AppliesTo(addActionEffect.action))
                return true;
        }
        return false;
    }

    public bool AppliesTo(Action a)
    {
        return (!filterByActionType || actionTypes.Contains(a.ActionType)) && a.SubActions.Any(SubActionFilter);
    }

    private bool SubActionFilter(SubAction sub)
    {
        return (!filterBySubType || sub.HasAnySubType(subTypes)) && AppliesTo(sub);
    }

    public abstract bool AppliesTo(SubAction sub);
}
