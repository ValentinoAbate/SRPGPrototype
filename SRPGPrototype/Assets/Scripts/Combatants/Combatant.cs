using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combatant : GridObject
{
    public abstract int MaxHp { get; }
    public abstract int Hp { get; protected set; }
    public bool Dead => Hp <= 0;
    public abstract int MaxAP { get; }
    public abstract int AP { get; protected set; }
    public abstract string DisplayName { get; }

    public virtual void Damage(int damage)
    {
        if (damage > Hp)
            Kill();
        else
            Hp -= damage;
    }

    public void Kill()
    {
        Hp = 0;
    }

    public bool CanUseAction(Action action) => AP >= action.APCost;
}
