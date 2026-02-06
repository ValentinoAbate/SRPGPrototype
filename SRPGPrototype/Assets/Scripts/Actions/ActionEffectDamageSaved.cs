using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectDamageSaved : ActionEffectDamageBasic
{
    public override bool CanSave(bool isBattle)
    {
        return true;
    }
    public override string Save(bool isBattle)
    {
        return damage.ToString();
    }

    public override void Load(string data, bool isBattle)
    {
        if(int.TryParse(data, out int savedDamage))
        {
            damage = savedDamage;
        }
    }
}
