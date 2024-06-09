using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Unit : GridObject
{
    public delegate void OnBattleStartDel(BattleGrid grid, Unit unit);

    public delegate void OnSubAction(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions);

    public delegate void OnAfterAction(Action action);

    public delegate void OnDeath(BattleGrid grid, Unit unit, Unit killedBy);

    public enum Team
    { 
        None,
        Enemy,
        Player,
        Obstacle,
        Data,
    }

    public enum Interference
    { 
        None,
        Low,
        Jamming
    }

    public virtual bool Movable => true;
    public abstract Team UnitTeam { get; }
    public abstract Interference InterferenceLevel { get; }
    public abstract int MaxHP { get; set; }
    public abstract int HP { get; protected set; }
    public bool Dead => HP <= 0;
    public abstract int MaxAP { get; set; }
    public abstract int AP { get; set; }
    public abstract int Repair { get; set; }
    public abstract CenterStat Power { get; }
    public abstract CenterStat Speed { get; }
    public abstract CenterStat Break { get; }
    public abstract OnSubAction OnBeforeSubActionFn { get; }
    public abstract OnSubAction OnAfterSubActionFn { get; }
    public abstract OnAfterAction OnAfterActionFn { get; }
    public abstract OnDeath OnDeathFn { get; }
    public abstract OnBattleStartDel OnBattleStartFn { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }

    public abstract Shell Shell { get; }

    public abstract IReadOnlyList<Action> Actions { get; }
    public abstract IReadOnlyList<ModifierActionDamage> IncomingDamageModifiers { get; }

    public void ModifyHp(BattleGrid grid, int amount, Unit source)
    {
        if (amount > 0)
            Heal(amount, source);
        else if (amount < 0)
            Damage(grid, -amount, source);
    }

    public virtual void Heal(int amount, Unit source)
    {
        if (Dead || amount <= 0)
            return;
        HP = Mathf.Min(HP + amount, MaxHP);
    }

    public virtual void Damage(BattleGrid grid, int amount, Unit source)
    {
        if (Dead)
            return;
        int damage = amount;
        if(!Break.IsZero)
        {
            damage += Break.Value;
            Break.Use();
        }
        if (damage <= 0)
            return;
        if (damage >= HP)
        {
            Kill(grid, source);
        }
        else
        {
            HP -= damage;
        }
    }

    public virtual void DoRepair()
    {
        HP += Repair;
        Repair = 0;
    }

    public void Kill(BattleGrid grid, Unit killedBy)
    {
        HP = 0;
        OnDeathFn?.Invoke(grid, this, killedBy);
        // On Death call may prevent death somehow
        if(Dead)
        {
            if(Pos != BattleGrid.OutOfBounds)
                grid.Remove(this);
            // Temporary: move to OOB
            transform.position = grid.GetSpace(BattleGrid.OutOfBounds);
        }
    }

    public virtual void ResetStats()
    {
        HP = MaxHP;
        AP = MaxAP;
        Power.Value = 0;
        Speed.Value = 0;
        Break.Value = 0;
    }

    public void ModifyStat(BattleGrid grid, Stats.StatName stat, int value, Unit source)
    {
        switch (stat)
        {
            case Stats.StatName.HP:
                ModifyHp(grid, value, source);
                break;
            case Stats.StatName.MaxHP:
                MaxHP += value;
                break;
            case Stats.StatName.AP:
                AP += value;
                break;
            case Stats.StatName.MaxAP:
                MaxAP += value;
                break;
            case Stats.StatName.Repair:
                Repair += value;
                break;
            case Stats.StatName.Power:
                Power.Value += value;
                break;
            case Stats.StatName.Speed:
                Speed.Value += value;
                break;
            case Stats.StatName.Break:
                Break.Value += value;
                break;
        }
    }

    public int GetStat(Stats.StatName stat)
    {
        switch (stat)
        {
            case Stats.StatName.HP:
                return HP;
            case Stats.StatName.MaxHP:
                return MaxHP;
            case Stats.StatName.AP:
                return AP;
            case Stats.StatName.MaxAP:
                return MaxAP;
            case Stats.StatName.Repair:
                return Repair;
            case Stats.StatName.Power:
                return Power.Value;
            case Stats.StatName.Speed:
                return Speed.Value;
            case Stats.StatName.Break:
                return Break.Value;
        }
        throw new System.Exception("Attempt to get invalid stat: " + stat.ToString());
    }

    public void SetStat(Stats.StatName stat, int value)
    {
        switch (stat)
        {
            case Stats.StatName.HP:
                HP = Mathf.Min(value, MaxHP);
                break;
            case Stats.StatName.MaxHP:
                MaxHP = value;
                break;
            case Stats.StatName.AP:
                AP = value;
                break;
            case Stats.StatName.MaxAP:
                MaxAP = value;
                break;
            case Stats.StatName.Repair:
                Repair = value;
                break;
            case Stats.StatName.Power:
                Power.Value = value;
                break;
            case Stats.StatName.Speed:
                Speed.Value = value;
                break;
            case Stats.StatName.Break:
                Break.Value = value;
                break;
        }
    }

    public bool CanUseAction(Action action)
    {
        return AP >= (action.APCost - Speed.Value);
    }

    public int ActionUsesUntilNoAP(Action action)
    {
        int uses = 0;
        int cost = action.APCost - Speed.Value;
        while(cost < AP)
        {
            ++uses;
            cost += action.APCostAfterXUses(uses) - Speed.ValueAfterXUses(uses);
        }
        return uses;
    }

    public virtual IEnumerator OnPhaseStart()
    {
        yield break;
    }

    public virtual IEnumerator OnPhaseEnd()
    {
        AP = MaxAP;
        Power.Value = 0;
        Speed.Value = 0;
        Break.Value = 0;
        yield break;
    }

    public virtual IEnumerator OnBattleStart(EncounterManager manager)
    {
        OnBattleStartFn?.Invoke(manager.grid, this);
        yield break;
    }

    public virtual IEnumerator OnBattleEnd(EncounterManager manager)
    {
        yield break;
    }
}
