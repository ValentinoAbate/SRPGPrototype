using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combatant : GridObject
{
    public enum Team
    { 
        None,
        Enemy,
        Player,
    }

    public abstract Team UnitTeam { get; }
    public abstract int MaxHP { get; }
    public abstract int HP { get; protected set; }
    public bool Dead => HP <= 0;
    public abstract int MaxAP { get; }
    public abstract int AP { get; protected set; }
    public abstract string DisplayName { get; }

    public abstract Shell Shell { get; }

    public abstract List<Action> Actions { get; }

    public virtual void Damage(int damage)
    {
        if (damage > HP)
            Kill();
        else
            HP -= damage;
    }

    public void Kill()
    {
        HP = 0;
    }

    public virtual void ResetStats()
    {
        HP = MaxHP;
        AP = MaxAP;
    }

    public bool CanUseAction(Action action) => AP >= action.APCost;
}
