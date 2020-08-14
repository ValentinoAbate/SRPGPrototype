using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Unit : GridObject
{
    public delegate void AbilityOnBattleStart(BattleGrid grid, Unit unit);

    public delegate void OnAfterSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Vector2Int> targetPositions);

    public delegate void OnDeath(BattleGrid grid, Unit unit, Unit deathCause);

    public enum Team
    { 
        None,
        Enemy,
        Player,
    }

    public virtual bool Movable => true;
    public abstract Team UnitTeam { get; }
    public abstract int MaxHP { get; set; }
    public abstract int HP { get; protected set; }
    public bool Dead => HP <= 0;
    public abstract int MaxAP { get; set; }
    public abstract int AP { get; set; }
    public abstract int Repair { get; set; }
    public abstract CenterStat Power { get; }
    public abstract CenterStat Speed { get; }
    public abstract CenterStat Defense { get; }
    public abstract OnAfterSubAction OnAfterSubActionFn { get; }
    public abstract OnDeath OnDeathFn { get; }
    public abstract string DisplayName { get; }

    public abstract Shell Shell { get; }

    public abstract List<Action> Actions { get; }

    public virtual void Heal(int amount, Unit source)
    {
        if (Dead || amount < 0)
            return;
        HP = Mathf.Min(HP + amount, MaxHP);
    }

    public virtual void Damage(BattleGrid grid, int damage, Unit source)
    {
        if (damage >= HP)
            Kill(grid, source);
        else
            HP -= damage;
    }

    public void Kill(BattleGrid grid, Unit killedBy)
    {
        HP = 0;
        grid.Remove(this);
        gameObject.SetActive(false);
        OnDeathFn?.Invoke(grid, this, killedBy);
    }

    public virtual void ResetStats()
    {
        HP = MaxHP;
        AP = MaxAP;
    }

    public bool CanUseAction(Action action)
    {
        return AP >= action.APCost;
    }

    public virtual IEnumerator OnPhaseStart()
    {
        yield break;
    }

    public virtual IEnumerator OnPhaseEnd()
    {
        AP = MaxAP;
        yield break;
    }

    public virtual IEnumerator OnBattleEnd(EncounterManager manager)
    {
        yield break;
    }
}
