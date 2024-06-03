using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddDamageModifierAbility : ProgramEffectAddAbility
{
    [SerializeField] private ModifierActionDamage modifier;
    [SerializeField] private string abilityName;
    [SerializeField] private bool incoming = true;

    protected override string AbilityName => abilityName;

    protected override void AddAbility(Program program, ref Shell.CompileData data)
    {
        if (!incoming)
        {
            return;
        }
        data.incomingDamageModifiers.Add(modifier);
    }
}
