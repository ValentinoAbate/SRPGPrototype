using UnityEngine;

public class ModifierActionDamage : ModifierAction
{
    public ActionEffectDamage[] DamageModifiers { get; private set; }
    [SerializeField] private GameObject actionEffectContainer;



    private void Awake()
    {
        if(actionEffectContainer != null)
        {
            DamageModifiers = actionEffectContainer.GetComponents<ActionEffectDamage>();
        }
        else
        {
            DamageModifiers = GetComponents<ActionEffectDamage>();
        }

    }

    public override bool AppliesTo(SubAction sub)
    {
        return sub.DealsDamage;
    }
}
